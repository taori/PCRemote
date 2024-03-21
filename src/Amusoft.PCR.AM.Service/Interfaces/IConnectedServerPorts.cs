using Amusoft.PCR.Domain.Service.Entities;

namespace Amusoft.PCR.AM.Service.Interfaces;

public interface IConnectedServerPorts
{
	ICollection<ServerConnection> Addresses { get; }
}