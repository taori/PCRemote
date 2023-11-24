namespace Amusoft.PCR.AM.Service.Features.GettingStarted;

public interface IAgentPingService
{
	public Task<bool> TryPingAsync();
}