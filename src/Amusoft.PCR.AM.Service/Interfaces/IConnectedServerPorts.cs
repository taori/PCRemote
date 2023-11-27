namespace Amusoft.PCR.AM.Service.Interfaces;

public interface IConnectedServerPorts
{
	ICollection<int> Addresses { get; }
}