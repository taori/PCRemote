using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using Amusoft.PCR.Domain.Shared.Entities;

namespace Amusoft.PCR.Int.IPC;

public class GrpcHandshakeFormatter
{
	public static string Write(string machineName, ServerConnection[] connections)
	{
		var portList = string.Join(";", connections.Select(c => $"{c.Protocol},{c.Port}"));
		return $"PCRemote 3 looking for clients at ports [{machineName}___{portList}]";
	}

	private static readonly Regex PortOnlyRegex = new("\\[(?<machine>[^_]+)___(?:(?<port>\\d+);?)+]", RegexOptions.Compiled | RegexOptions.Singleline);
	private static readonly Regex PortWithProtocolRegex = new("\\[(?<machine>[^_]+)___(?:(?<protocol>\\w+),(?<port>\\d+);?)+]", RegexOptions.Compiled | RegexOptions.Singleline);

	public static GrpcHandshakeMessage? Parse(byte[] message)
	{
		var content = Encoding.UTF8.GetString(message);
		if (TryParsePortAndProtocols(content, out var matches, out var machineName))
		{
			return new GrpcHandshakeMessage(machineName, matches);
		}

		// fallback 3.0.0
		if (TryParsePorts(content, out matches, out machineName))
		{
			return new GrpcHandshakeMessage(machineName, matches);
		}

		return null;
	}

	private static bool TryParsePorts(string content, out ServerConnection[] matches, [NotNullWhen(true)] out string? machineName)
	{
		matches = Array.Empty<ServerConnection>();
		machineName = null;
		var match = PortOnlyRegex.Match(content);
		if (match.Success)
		{
			machineName = match.Groups["machine"].Value;
			var portValues = match.Groups["port"].Captures.Select(d => d.Value);
			matches = portValues
				.Select(port => new ServerConnection(int.Parse(port), "http"))
				.ToArray();
		}

		return match.Success;
	}

	private static bool TryParsePortAndProtocols(string content, out ServerConnection[] matches, [NotNullWhen(true)] out string? machineName)
	{
		matches = Array.Empty<ServerConnection>();
		machineName = null;
		var match = PortWithProtocolRegex.Match(content);
		if (match.Success)
		{
			machineName = match.Groups["machine"].Value;
			var portValues = match.Groups["port"].Captures.Select(d => d.Value).ToArray();
			var protocolValues = match.Groups["protocol"].Captures.Select(d => d.Value).ToArray();
			matches = portValues
				.Select((portValue, index) => new ServerConnection(int.Parse(portValue), protocolValues[index]))
				.ToArray();
		}

		return match.Success;
	}
}

public static class GrpcHandshakeClientMessage
{
	public const string Message = "Client looking for hosts";
}

public readonly struct GrpcHandshakeMessage
{
	public GrpcHandshakeMessage(string machineName, ServerConnection[] connections)
	{
		MachineName = machineName;
		Connections = connections;
	}

	public string MachineName { get; }
	public ServerConnection[] Connections { get; }
}