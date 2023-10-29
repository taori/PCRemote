namespace Amusoft.PCR.Application.Features.DesktopIntegration;

public interface IConnectedServerPorts
{
	ICollection<int> Addresses { get; }
}