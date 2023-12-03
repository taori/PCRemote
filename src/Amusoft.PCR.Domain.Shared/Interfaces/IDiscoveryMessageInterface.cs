using System.Diagnostics.CodeAnalysis;

namespace Amusoft.PCR.Domain.Shared.Interfaces;

public interface IDiscoveryMessageInterface
{
	bool IsRespondableMessage(string message);
	string GetResponseMessage(string machineName, int[] ports);
	bool TryParse(byte[] content, [NotNullWhen(true)] out (string MachineName, int[] Ports)? value);
	string DiscoveryRequestMessage { get; }
}