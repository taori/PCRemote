using Amusoft.PCR.AM.Service.Features;
using Amusoft.PCR.AM.Service.Features.GettingStarted;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.AM.Service.Extensions;

public static class ServiceCollectionExtensions
{
	public static void AddApplicationModel(this IServiceCollection source)
	{
		source.AddScoped<ServerEndpointProvider>();

		source.AddSingleton<IAgentPingService, AgentPingService>();
		source.AddHttpClient();
	}
}