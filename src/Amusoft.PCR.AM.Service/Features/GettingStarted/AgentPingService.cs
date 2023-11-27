using Amusoft.PCR.Int.IPC;

namespace Amusoft.PCR.AM.Service.Features.GettingStarted;

public class AgentPingService : IAgentPingService
{
	private readonly DesktopIntegrationService.DesktopIntegrationServiceClient _client;

	public AgentPingService(Int.IPC.DesktopIntegrationService.DesktopIntegrationServiceClient client)
	{
		_client = client;
	}

	public async Task<bool> TryPingAsync()
	{
		try
		{
			var r = await _client.PingAsync(new DefaultRequest());
			return r.Success;
		}
		catch (Exception)
		{
			return false;
		}
	}
}