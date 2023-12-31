﻿using Amusoft.PCR.AM.Shared.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels;
using Amusoft.PCR.App.UI.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Amusoft.PCR.AM.UI;

public static class ServiceCollectionExtensions
{
	public static void AddUIApplicationModel(this IServiceCollection services)
	{
		services.TryAddTransient<INestedServiceProviderFactory, NestedServiceProviderFactory>();

		services.AddSingleton<DebugViewModel>();
		services.AddSingleton<HostsOverviewViewModel>();
		services.AddSingleton<HostAccountCreationViewModel>();
		services.AddSingleton<MainViewModel>();
		services.AddSingleton<LogsViewModel>();
		services.AddSingleton<LogSettingsViewModel>();
		services.AddSingleton<SettingsViewModel>();

		services.TryAddTransient<HostAccountPermissionsViewModel>();
		services.TryAddTransient<HostAccountsViewModel>();
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