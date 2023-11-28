using System.Diagnostics;
using Amusoft.PCR.AM.Shared.Interfaces;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.Repositories;
using Amusoft.PCR.AM.UI.ViewModels;
using Amusoft.PCR.App.UI.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.AM.UI.Extensions;

public static class ServiceProviderExtensions
{
	public static void AddUIApplicationModel(this IServiceCollection services)
	{
		// services.AddSingleton<INavigation, Navigation>();
		// services.AddSingleton<ITypedNavigator, TypedNavigator>();
		// services.AddSingleton<IToast, Toast>();
		// services.AddSingleton<IAgentEnvironment, AgentEnvironment>();
		// services.AddSingleton<IUserInterfaceService, UserInterfaceService>();
		// services.AddSingleton<IFileStorage, FileStorage>();
		// services.AddSingleton<GrpcChannelFactory>();
		//
		// services.AddSingleton<HostRepository>();
		// services.AddSingleton<ClientSettingsRepository>();
		//
		// services.AddSingleton<IDesktopIntegrationServiceFactory, DesktopIntegrationServiceFactory>();
		services.AddSingleton<INestedServiceProviderFactory, NestedServiceProviderFactory>();
		
		services.AddTransient<CommandButtonListViewModel>();
		services.AddTransient<MouseControlViewModel>();
		
		services.AddScoped<DebugViewModel>();
		services.AddScoped<MainViewModel>();
		services.AddScoped<LogsViewModel>();
		services.AddScoped<AudioViewModel>();
		services.AddScoped<ProgramsViewModel>();
		services.AddScoped<InputControlViewModel>();
		services.AddScoped<MonitorsViewModel>();
		services.AddScoped<SystemStateViewModel>();
		services.AddScoped<HostsOverviewViewModel>();
		services.AddScoped<SettingsViewModel>();
		services.AddScoped<HostViewModel>();
	}
}