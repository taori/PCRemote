using Amusoft.PCR.App.UI.Implementations;
using Amusoft.PCR.App.UI.Pages;
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
		services.AddSingleton<IFileStorage, FileStorage>();
		services.AddSingleton<HostRepository>();

		services.AddScoped<MainPage>();
		services.AddScoped<MainViewModel>();

		services.AddScoped<Audio>();
		services.AddScoped<AudioViewModel>();

		services.AddScoped<HostsOverview>();
		services.AddScoped<HostsOverviewViewModel>();

		services.AddScoped<Settings>();
		services.AddScoped<SettingsViewModel>();

		services.AddScoped<Host>();
		services.AddScoped<HostViewModel>();
	}
}