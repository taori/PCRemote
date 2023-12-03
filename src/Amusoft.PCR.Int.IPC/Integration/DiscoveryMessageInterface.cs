using System.Diagnostics.CodeAnalysis;
using Amusoft.PCR.Domain.Shared.Interfaces;

namespace Amusoft.PCR.Int.IPC.Integration;

internal class DiscoveryMessageInterface : IDiscoveryMessageInterface
{
	public bool IsRespondableMessage(string message)
	{
		return string.Equals(message, GrpcHandshakeClientMessage.Message);
	}

	public string GetResponseMessage(string machineName, int[] ports)
	{
		return GrpcHandshakeFormatter.Write(machineName, ports);
	}

	public bool TryParse(byte[] content, [NotNullWhen(true)] out (string MachineName, int[] Ports)? value)
	{
		var parsed = GrpcHandshakeFormatter.Parse(content);
		if (parsed is { } p)
		{
			value = (p.MachineName, p.Ports);
		}
		else
		{
			value = null;
		}
		
		return parsed != null;
	}

	public string DiscoveryRequestMessage => GrpcHandshakeClientMessage.Message;
}