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
		return SpawnPushConfigureAsync<Host, HostViewModel>(configureModel);
	}

	public Task OpenHostOverview()
	{
		return SpawnPushAsync<HostsOverview, HostsOverviewViewModel>();
	}

	public Task OpenSettings()
	{
		return SpawnPushAsync<Settings, SettingsViewModel>();
	}

	public Task OpenAudio()
	{
		return SpawnPushAsync<Audio, AudioViewModel>();
	}

	public Task OpenSystemState()
	{
		return SpawnPushAsync<SystemState, SystemStateViewModel>();
	}

	public Task OpenMonitors()
	{
		return SpawnPushAsync<Monitors, MonitorsViewModel>();
	}

	public Task OpenInputControl()
	{
		return SpawnPushAsync<InputControl, InputControlViewModel>();
	}

	public Task OpenMouseControl()
	{
		return SpawnPushAsync<MouseControl, MouseControlViewModel>();
	}

	public Task OpenPrograms()
	{
		return SpawnPushAsync<Programs, ProgramsViewModel>();
	}

	public Task ScopedNavigationAsync(Action<IServiceCollection> scopeConfiguration, Func<ITypedNavigator, Task> navigate)
	{
		var provider = _nestedServiceProviderFactory.FromCurrentScope(scopeConfiguration);
		var navigator = provider.GetRequiredService<ITypedNavigator>();
		return navigate(navigator);
	}

	private Task SpawnPushAsync<TPage, TViewModel>() 
		where TPage : Page 
		where TViewModel : notnull
	{
		return SpawnPushConfigureAsync<TPage, TViewModel>(null);
	}

	private Task SpawnPushConfigureAsync<TPage, TViewModel>(Action<TViewModel>? configure) 
		where TPage : Page where TViewModel : notnull
	{
		var spawn = SpawnPageAndModel<TPage, TViewModel>();
		configure?.Invoke(spawn.viewModel);
		return Shell.Current.Navigation.PushAsync(spawn.page);
	}

	private (TPage page, TViewModel viewModel) SpawnPageAndModel<TPage, TViewModel>() 
		where TPage : Page
		where TViewModel : notnull
	{
		using var scope = _serviceProvider.CreateScope();
		var model = scope.ServiceProvider.GetRequiredService<TViewModel>();
		var page = scope.ServiceProvider.GetRequiredService<TPage>();
		page.BindingContext = model;

		return (page, model);
	}
}