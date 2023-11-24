using Amusoft.PCR.AM.Service.Features.GettingStarted;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Amusoft.PCR.App.Service.HealthChecks;

public class AgentConnectivityCheck : IHealthCheck
{
	private readonly IAgentPingService _agentPingService;

	public AgentConnectivityCheck(IAgentPingService agentPingService)
	{
		_agentPingService = agentPingService;
	}
	
	public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
	{
		return await _agentPingService.TryPingAsync() 
			? HealthCheckResult.Healthy() 
			: HealthCheckResult.Unhealthy("Unable to connect to Agent");
	}
}