using System.Diagnostics.CodeAnalysis;
using Amusoft.PCR.Domain.Shared.Entities;

namespace Amusoft.PCR.Domain.Shared.Interfaces;

public interface IDiscoveryMessageInterface
{
	bool IsRespondableMessage(string message);
	string GetResponseMessage(string machineName, ServerConnection[] connections);
	bool TryParse(byte[] content, [NotNullWhen(true)] out (string MachineName, ServerConnection[] Connections)? value);
	string DiscoveryRequestMessage { get; }
}