using Amusoft.PCR.App.UI.Implementations;
using Amusoft.PCR.App.UI.Pages;
using Amusoft.PCR.Application.Features.DesktopIntegration;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.UI.Repos;
using Amusoft.PCR.Application.UI.VM;
using Amusoft.PCR.Domain.Services;

namespace Amusoft.PCR.App.UI;

public static class MauiServiceRegistrar
{
	public static void Register(IServiceCollection services)
	{
		services.AddSingleton<Amusoft.PCR.Domain.Services.INavigation, Navigation>();
		services.AddSingleton<ITypedNavigator, TypedNavigator>();
		services.AddSingleton<IToast, Toast>();
		services.AddSingleton<IAgentEnvironment, AgentEnvironment>();
		services.AddSingleton<IUserInterfaceService, UserInterfaceService>();
		services.AddSingleton<IFileStorage, FileStorage>();

		services.AddSingleton<HostRepository>();
		services.AddSingleton<ClientSettingsRepository>();

		services.AddSingleton<IDesktopIntegrationServiceFactory, DesktopIntegrationServiceFactory>();
		services.AddSingleton<INestedServiceProviderFactory, NestedServiceProviderFactory>();

		services.AddTransient<CommandButtonList>();
		services.AddTransient<CommandButtonListViewModel>();

		services.AddScoped<MainPage>();
		services.AddScoped<MainViewModel>();

		services.AddScoped<Audio>();
		services.AddScoped<AudioViewModel>();

		services.AddScoped<Programs>();
		services.AddScoped<ProgramsViewModel>();

		services.AddScoped<InputControl>();
		services.AddScoped<InputControlViewModel>();

		services.AddScoped<MouseControl>();
		services.AddScoped<MouseControlViewModel>();

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