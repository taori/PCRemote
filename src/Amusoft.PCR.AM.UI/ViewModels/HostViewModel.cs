using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class HostViewModel : PageViewModel, INavigationCallbacks
{
	private readonly ITypedNavigator _navigator;
	private readonly IDesktopIntegrationServiceFactory _integrationServiceFactory;

	public Task OnNavigatedToAsync()
	{
		return Task.CompletedTask;
	}
	public IIpcIntegrationService IpcClient { get; }

	protected override string GetDefaultPageTitle()
	{
		return "default";
	}

	[RelayCommand]
	public Task OpenAudio()
	{
		return _navigator.ScopedNavigationAsync(d => d.AddSingleton(this), d => d.OpenAudio());
	}

	[RelayCommand]
	public Task OpenSystemState()
	{
		return _navigator.ScopedNavigationAsync(d => d.AddSingleton(this), d => d.OpenSystemState());
	}

	[RelayCommand]
	public Task OpenMonitors()
	{
		return _navigator.ScopedNavigationAsync(d => d.AddSingleton(this), d => d.OpenMonitors());
	}

	[RelayCommand]
	public Task OpenInputControl()
	{
		return _navigator.ScopedNavigationAsync(d => d.AddSingleton(this), d => d.OpenInputControl());
	}

	[RelayCommand]
	public Task OpenPrograms()
	{
		return _navigator.ScopedNavigationAsync(d => d.AddSingleton(this), d => d.OpenPrograms());
	}

	public HostViewModel(ITypedNavigator navigator, IDesktopIntegrationServiceFactory integrationServiceFactory, IHostCredentialProvider credentialProvider) : base(navigator)
	{
		_navigator = navigator;
		_integrationServiceFactory = integrationServiceFactory;
		IpcClient = _integrationServiceFactory.Create("http", credentialProvider.Address);
		Title = credentialProvider.Title;
	}
}