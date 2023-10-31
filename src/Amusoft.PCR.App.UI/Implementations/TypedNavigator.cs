#nullable enable
using Amusoft.PCR.App.UI.Pages;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.UI.VM;

namespace Amusoft.PCR.App.UI.Implementations;

public class TypedNavigator : ITypedNavigator
{
	private readonly IServiceProvider _serviceProvider;

	public TypedNavigator(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
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
		where TPage : notnull 
		where TViewModel : notnull
	{
		using var scope = _serviceProvider.CreateScope();
		return (scope.ServiceProvider.GetRequiredService<TPage>(), scope.ServiceProvider.GetRequiredService<TViewModel>());
	}
}