#region

using System.Net;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using Amusoft.PCR.Domain.UI.Entities;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class HostViewModel : PageViewModel, INavigationCallbacks
{
	private readonly IBearerTokenStorage _bearerTokenStorage;
	private readonly ITypedNavigator _navigator;
	private readonly IDesktopIntegrationServiceFactory _integrationServiceFactory;

	public async Task OnNavigatedToAsync()
	{
#if DEBUG

		// todo remove
		var bearerToken = new BearerToken("asdf", "access", "refresh", DateTimeOffset.Now);
		await _bearerTokenStorage.AddTokenAsync(new IPEndPoint(IPAddress.Broadcast, 80), bearerToken, CancellationToken.None);
		await _bearerTokenStorage.PruneAsync(new IPEndPoint(IPAddress.Broadcast, 80), CancellationToken.None);

#endif
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

	public HostViewModel(IBearerTokenStorage bearerTokenStorage, ITypedNavigator navigator, IDesktopIntegrationServiceFactory integrationServiceFactory, IHostCredentialProvider credentialProvider) : base(navigator)
	{
		_bearerTokenStorage = bearerTokenStorage;
		_navigator = navigator;
		_integrationServiceFactory = integrationServiceFactory;
		IpcClient = _integrationServiceFactory.Create(credentialProvider.Protocol, credentialProvider.Address);
		Title = credentialProvider.Title;
	}
}