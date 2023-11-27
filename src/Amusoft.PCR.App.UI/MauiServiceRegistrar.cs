using Amusoft.PCR.AM.Shared.Services;
using Amusoft.PCR.App.UI.Implementations;
using Amusoft.PCR.App.UI.Pages;
using Amusoft.PCR.Application.Features.DesktopIntegration;
using Amusoft.PCR.Application.UI.Repos;
using Amusoft.PCR.Application.UI.VM;
using Amusoft.PCR.Domain.Services;

namespace Amusoft.PCR.App.UI;

public static class MauiServiceRegistrar
{
	public static void Register(IServiceCollection services)
	{
		// This switch must be set before creating the GrpcChannel/HttpClient.
		// AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

		services.AddSingleton<Amusoft.PCR.Domain.Services.INavigation, Navigation>();
		services.AddSingleton<ITypedNavigator, TypedNavigator>();
		services.AddSingleton<IToast, Toast>();
		services.AddSingleton<IAgentEnvironment, AgentEnvironment>();
		services.AddSingleton<IUserInterfaceService, UserInterfaceService>();
		services.AddSingleton<IFileStorage, FileStorage>();
		services.AddSingleton<GrpcChannelFactory>();

		services.AddSingleton<HostRepository>();
		services.AddSingleton<ClientSettingsRepository>();

		services.AddSingleton<IDesktopIntegrationServiceFactory, DesktopIntegrationServiceFactory>();
		services.AddSingleton<INestedServiceProviderFactory, NestedServiceProviderFactory>();

		services.AddTransient<CommandButtonList>();
		services.AddTransient<CommandButtonListViewModel>();

		services.AddScoped<Debug>();
		services.AddScoped<DebugViewModel>();

		services.AddScoped<MainPage>();
		services.AddScoped<MainViewModel>();

		services.AddScoped<Logs>();
		services.AddScoped<LogsViewModel>();

		services.AddScoped<Audio>();
		services.AddScoped<AudioViewModel>();

		services.AddScoped<Programs>();
		services.AddScoped<ProgramsViewModel>();

		services.AddScoped<InputControl>();
		services.AddScoped<InputControlViewModel>();

		services.AddTransient<MouseControl>();
		services.AddTransient<MouseControlViewModel>();

		services.AddScoped<Monitors>();
		services.AddScoped<MonitorsViewModel>();

		services.AddScoped<SystemState>();
		services.AddScoped<SystemStateViewModel>();

		services.AddScoped<HostsOverview>();
		services.AddScoped<HostsOverviewViewModel>();

		services.AddScoped<Settings>();
		services.AddScoped<SettingsViewModel>();

		services.AddScoped<Host>();
		services.AddScoped<HostViewModel>();
	}
}