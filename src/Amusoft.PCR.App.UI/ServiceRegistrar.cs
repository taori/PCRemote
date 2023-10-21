using Amusoft.PCR.App.UI.Implementations;
using Amusoft.PCR.App.UI.Pages;
using Amusoft.PCR.Application.UI.Repos;
using Amusoft.PCR.Application.UI.VM;
using Amusoft.PCR.Domain.Services;

namespace Amusoft.PCR.App.UI;

public static class ServiceRegistrar
{
	public static void Register(IServiceCollection services)
	{
		services.AddSingleton<Amusoft.PCR.Domain.Services.INavigation, Navigation>();
		services.AddSingleton<IToast, Toast>();
		services.AddSingleton<IFileStorage, FileStorage>();
		services.AddSingleton<HostRepository>();

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