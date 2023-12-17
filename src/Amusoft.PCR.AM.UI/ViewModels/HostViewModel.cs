using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class HostViewModel : PageViewModel, INavigationCallbacks
{
	private readonly ITypedNavigator _navigator;

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
	private Task OpenAccounts()
	{
		return _navigator.ScopedNavigationAsync(d => d.AddSingleton(this), d => d.OpenHostAccounts());
	}

	[RelayCommand]
	private Task OpenAudio()
	{
		return _navigator.ScopedNavigationAsync(d => d.AddSingleton(this), d => d.OpenAudio());
	}

	[RelayCommand]
	private Task OpenSystemState()
	{
		return _navigator.ScopedNavigationAsync(d => d.AddSingleton(this), d => d.OpenSystemState());
	}

	[RelayCommand]
	private Task OpenMonitors()
	{
		return _navigator.ScopedNavigationAsync(d => d.AddSingleton(this), d => d.OpenMonitors());
	}

	[RelayCommand]
	private Task OpenInputControl()
	{
		return _navigator.ScopedNavigationAsync(d => d.AddSingleton(this), d => d.OpenInputControl());
	}

	[RelayCommand]
	private Task OpenPrograms()
	{
		return _navigator.ScopedNavigationAsync(d => d.AddSingleton(this), d => d.OpenPrograms());
	}

	public HostViewModel(ITypedNavigator navigator, IDesktopIntegrationServiceFactory integrationServiceFactory, IHostCredentialProvider credentialProvider) : base(navigator)
	{
		_navigator = navigator;
		IpcClient = integrationServiceFactory.Create(credentialProvider.Protocol, credentialProvider.Address);
		Title = credentialProvider.Title;
	}
}