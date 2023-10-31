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
		var spawn = SpawnPageAndModel<Host, HostViewModel>();
		configureModel(spawn.viewModel);
		return Shell.Current.Navigation.PushAsync(spawn.page);
	}

	public Task OpenHostOverview()
	{
		var spawn = SpawnPageAndModel<HostsOverview, HostsOverviewViewModel>();
		return Shell.Current.Navigation.PushAsync(spawn.page);
	}

	private (TPage page, TViewModel viewModel) SpawnPageAndModel<TPage, TViewModel>()
	{
		using var scope = _serviceProvider.CreateScope();
		return (scope.ServiceProvider.GetRequiredService<TPage>(), scope.ServiceProvider.GetRequiredService<TViewModel>());
	}
}