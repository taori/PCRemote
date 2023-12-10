#region

using Amusoft.PCR.AM.Shared.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels;
using Amusoft.PCR.App.UI.Implementations;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace Amusoft.PCR.AM.UI.Extensions;

public static class ServiceProviderExtensions
{
	public static void AddUIApplicationModel(this IServiceCollection services)
	{
		services.AddSingleton<INestedServiceProviderFactory, NestedServiceProviderFactory>();

		services.AddSingleton<DebugViewModel>();
		services.AddSingleton<HostsOverviewViewModel>();
		services.AddSingleton<MainViewModel>();
		services.AddSingleton<LogsViewModel>();
		services.AddSingleton<SettingsViewModel>();

		services.AddScoped<CommandButtonListViewModel>();
		services.AddScoped<MouseControlViewModel>();
		services.AddScoped<AudioViewModel>();
		services.AddScoped<ProgramsViewModel>();
		services.AddScoped<InputControlViewModel>();
		services.AddScoped<MonitorsViewModel>();
		services.AddScoped<SystemStateViewModel>();
		services.AddScoped<HostViewModel>();
	}
}