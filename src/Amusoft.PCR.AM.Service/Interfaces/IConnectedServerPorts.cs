using Amusoft.PCR.Domain.Shared.Entities;

namespace Amusoft.PCR.AM.Service.Interfaces;

public interface IConnectedServerPorts
{
	ICollection<ServerConnection> Addresses { get; }
}