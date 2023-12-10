#region

using Amusoft.PCR.AM.Shared.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels;
using Amusoft.PCR.App.UI.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

#endregion

namespace Amusoft.PCR.AM.UI.Extensions;

public static class ServiceProviderExtensions
{
	public static void AddUIApplicationModel(this IServiceCollection services)
	{
		services.TryAddTransient<INestedServiceProviderFactory, NestedServiceProviderFactory>();

		services.AddSingleton<DebugViewModel>();
		services.AddSingleton<HostsOverviewViewModel>();
		services.AddSingleton<MainViewModel>();
		services.AddSingleton<LogsViewModel>();
		services.AddSingleton<SettingsViewModel>();

		services.TryAddTransient<CommandButtonListViewModel>();
		services.TryAddTransient<MouseControlViewModel>();
		services.TryAddTransient<AudioViewModel>();
		services.TryAddTransient<ProgramsViewModel>();
		services.TryAddTransient<InputControlViewModel>();
		services.TryAddTransient<MonitorsViewModel>();
		services.TryAddTransient<SystemStateViewModel>();
		services.TryAddTransient<HostViewModel>();
	}
}