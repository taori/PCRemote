using Amusoft.PCR.App.UI.Implementations;
using Amusoft.PCR.App.UI.Pages;
using Amusoft.PCR.Application.UI;
using Amusoft.PCR.Domain.Services;
using INavigation = Amusoft.PCR.Domain.Services.INavigation;

namespace Amusoft.PCR.App.UI;

public static class ServiceRegistrar
{
	public static void Register(IServiceCollection services)
	{
		services.AddSingleton<INavigation, Navigation>();
		services.AddSingleton<IToast, Toast>();

		services.AddTransient<MainPage>();
		services.AddTransient<MainViewModel>();

		services.AddTransient<Audio>();
		services.AddTransient<AudioViewModel>();

		services.AddTransient<HostsOverview>();
		services.AddTransient<HostsOverviewViewModel>();

		services.AddTransient<Settings>();
		services.AddTransient<SettingsViewModel>();
	}
}