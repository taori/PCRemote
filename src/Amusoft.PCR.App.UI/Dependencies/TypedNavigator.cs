#nullable enable
using System.Net;
using Amusoft.PCR.AM.Shared.Interfaces;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels;
using Amusoft.PCR.App.UI.Dependencies;
using Amusoft.PCR.App.UI.Pages;

namespace Amusoft.PCR.App.UI.Implementations;

public class TypedNavigator : ITypedNavigator
{
	private readonly IServiceProvider _serviceProvider;
	private readonly INestedServiceProviderFactory _nestedServiceProviderFactory;

	public TypedNavigator(IServiceProvider serviceProvider, INestedServiceProviderFactory nestedServiceProviderFactory)
	{
		_serviceProvider = serviceProvider;
		_nestedServiceProviderFactory = nestedServiceProviderFactory;
	}

	public Task PopAsync()
	{
		return Shell.Current.Navigation.PopAsync();
	}

	public Task OpenHost(IPEndPoint endPoint, string title)
	{
		var provider = _nestedServiceProviderFactory.FromCurrentScope(collection => collection.AddSingleton<IHostCredentialProvider>(new EndpointData(endPoint, title)));
		return SpawnPushAsync<Host, HostViewModel>(provider);
	}

	public Task OpenHostOverview()
	{
		return SpawnPushAsync<HostsOverview, HostsOverviewViewModel>(_serviceProvider);
	}

	public Task OpenSettings()
	{
		return SpawnPushAsync<Settings, SettingsViewModel>(_serviceProvider);
	}

	public Task OpenAudio()
	{
		return SpawnPushAsync<Audio, AudioViewModel>(_serviceProvider);
	}

	public Task OpenSystemState()
	{
		return SpawnPushAsync<SystemState, SystemStateViewModel>(_serviceProvider);
	}

	public Task OpenMonitors()
	{
		return SpawnPushAsync<Monitors, MonitorsViewModel>(_serviceProvider);
	}

	public Task OpenInputControl()
	{
		return SpawnPushAsync<InputControl, InputControlViewModel>(_serviceProvider);
	}

	public Task OpenMouseControl()
	{
		return SpawnPushAsync<MouseControl, MouseControlViewModel>(_serviceProvider);
	}

	public Task OpenPrograms()
	{
		return SpawnPushAsync<Programs, ProgramsViewModel>(_serviceProvider);
	}

	public Task OpenLogs()
	{
		return SpawnPushAsync<Logs, LogsViewModel>(_serviceProvider);
	}

	public Task OpenDebug()
	{
		return SpawnPushAsync<Debug, DebugViewModel>(_serviceProvider);
	}

	public Task OpenCommandButtonList(Action<CommandButtonListViewModel> configure, HostViewModel host)
	{
		var provider = _nestedServiceProviderFactory.FromCurrentScope(collection => collection.AddSingleton(host));
		return SpawnPushConfigureAsync<CommandButtonList, CommandButtonListViewModel>(provider, configure);
	}

	public Task ScopedNavigationAsync(Action<IServiceCollection> scopeConfiguration, Func<ITypedNavigator, Task> navigate)
	{
		var provider = _nestedServiceProviderFactory.FromCurrentScope(scopeConfiguration);
		var navigator = provider.GetRequiredService<ITypedNavigator>();
		return navigate(navigator);
	}

	private Task SpawnPushAsync<TPage, TViewModel>(IServiceProvider serviceProvider) 
		where TPage : Page 
		where TViewModel : notnull
	{
		return SpawnPushConfigureAsync<TPage, TViewModel>(serviceProvider, null);
	}

	private Task SpawnPushConfigureAsync<TPage, TViewModel>(IServiceProvider serviceProvider, Action<TViewModel>? configure) 
		where TPage : Page 
		where TViewModel : notnull
	{
		var spawn = SpawnPageAndModel<TPage, TViewModel>(serviceProvider);
		configure?.Invoke(spawn.viewModel);
		return Shell.Current.Navigation.PushAsync(spawn.page);
	}

	private (TPage page, TViewModel viewModel) SpawnPageAndModel<TPage, TViewModel>(IServiceProvider serviceProvider) 
		where TPage : Page
		where TViewModel : notnull
	{
		using var scope = serviceProvider.CreateScope();
		var model = scope.ServiceProvider.GetRequiredService<TViewModel>();
		var page = scope.ServiceProvider.GetRequiredService<TPage>();
		page.BindingContext = model;

		return (page, model);
	}
}