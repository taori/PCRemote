using System.Diagnostics.CodeAnalysis;
using Amusoft.PCR.Domain.Shared.Entities;
using Amusoft.PCR.Domain.Shared.Interfaces;

namespace Amusoft.PCR.Int.IPC.Integration;

internal class DiscoveryMessageInterface : IDiscoveryMessageInterface
{
	public bool IsRespondableMessage(string message)
	{
		return string.Equals(message, GrpcHandshakeClientMessage.Message);
	}

	public string GetResponseMessage(string machineName, ServerConnection[] connections)
	{
		return GrpcHandshakeFormatter.Write(machineName, connections);
	}

	public bool TryParse(byte[] content, [NotNullWhen(true)] out (string MachineName, ServerConnection[] Connections)? value)
	{
		var parsed = GrpcHandshakeFormatter.Parse(content);
		value = null;
		if (parsed is null)
			return false;

		value = (parsed.Value.MachineName, parsed.Value.Connections);
		return true;
	}

	public string DiscoveryRequestMessage => GrpcHandshakeClientMessage.Message;
}