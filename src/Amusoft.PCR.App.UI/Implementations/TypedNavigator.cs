#nullable enable
using Amusoft.PCR.App.UI.Pages;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.UI.VM;

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

	public Task OpenHost(Action<HostViewModel> configureModel)
	{
		return SpawnPushConfigureAsync<Host, HostViewModel>(_serviceProvider, configureModel);
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