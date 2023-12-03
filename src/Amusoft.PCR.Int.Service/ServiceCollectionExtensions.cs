using Amusoft.PCR.AM.Service.Features.GettingStarted;
using Amusoft.PCR.AM.Service.Interfaces;
using Amusoft.PCR.AM.Shared;
using Amusoft.PCR.AM.Shared.Interfaces;
using Amusoft.PCR.Domain.Shared.Interfaces;
using Amusoft.PCR.Int.IPC;
using Amusoft.PCR.Int.IPC.Integration;
using Amusoft.PCR.Int.Service.Services;
using GrpcDotNetNamedPipes;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.Int.Service;

public static class ServiceCollectionExtensions
{
	public static void AddServiceIntegration(this IServiceCollection services)
	{
		services.AddInterprocessCommunication();
		
		services.AddSingleton<IBroadcastCommunicatorFactory, BroadcastCommunicatorFactory>();
		services.AddSingleton<IImpersonatedProcessLauncher, ImpersonatedProcessLauncher>();
		services.AddSingleton<IDesktopClientMethods, DesktopServiceClientWrapper>();
		services.AddSingleton<IAgentPingService, AgentPingService>();
		
		services.AddSingleton<Int.IPC.DesktopIntegrationService.DesktopIntegrationServiceClient>(provider =>
		{
			return new Int.IPC.DesktopIntegrationService.DesktopIntegrationServiceClient(provider.GetRequiredService<NamedPipeChannel>());
		});
		services.AddSingleton<NamedPipeChannel>(d => new NamedPipeChannel(".", Globals.NamedPipeChannel));
	}
}