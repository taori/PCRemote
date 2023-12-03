using Amusoft.PCR.AM.Shared.Interfaces;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.Repositories;
using Amusoft.PCR.App.UI.Implementations;
using Amusoft.PCR.Int.IPC;
using Amusoft.PCR.Int.UI.ProjectDepencies;
using Amusoft.PCR.Int.UI.Shared;

namespace Amusoft.PCR.Int.UI;

public static class ServiceCollectionExtensions
{
	public static void AddUIIntegration(this IServiceCollection services)
	{
		services.AddInterprocessCommunication();
		
		services.AddSingleton<IToast, Toast>();
		services.AddSingleton<IAgentEnvironment, AgentEnvironment>();
		services.AddSingleton<IUserInterfaceService, UserInterfaceService>();
		services.AddSingleton<IFileStorage, FileStorage>();
		services.AddSingleton<IGrpcChannelFactory, GrpcChannelFactory>();

		services.AddSingleton<IHostRepository, HostRepository>();
		services.AddSingleton<IClientSettingsRepository, ClientSettingsRepository>();

		services.AddSingleton<IDesktopIntegrationServiceFactory, DesktopIntegrationServiceFactory>();
	}
}