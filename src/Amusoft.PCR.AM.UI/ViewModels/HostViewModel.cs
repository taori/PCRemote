using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class HostViewModel : PageViewModel, INavigationCallbacks
{
	private readonly ITypedNavigator _navigator;
	private readonly IHostCredentialProvider _credentialProvider;
	private readonly IEndpointAccountManager _endpointAccountManager;
	private readonly IUserInterfaceService _userInterfaceService;
	public IIpcIntegrationService IpcClient { get; }

	public async Task OnNavigatedToAsync()
	{
		var endpointAccountId = await _endpointAccountManager.GetEndpointAccountIdAsync(_credentialProvider.Address);
		if (endpointAccountId is null)
		{
			var userDecision = await _userInterfaceService.DisplayConfirmAsync(Translations.Generic_ActionRequired, Translations.Host_AccountRequiredToInteractWithHost);
			if (userDecision)
				await _endpointAccountManager.GetEndpointAccountIdAsync(_credentialProvider.Address);
		}
	}

	public async Task OnNavigatingAsync(INavigatingContext context)
	{
		if (context.NavigationKind == NavigationKind.Push)
		{
			await using var scope = context.PauseNavigation();
			var endpointAccountId = await _endpointAccountManager.GetEndpointAccountIdAsync(_credentialProvider.Address);
			if (endpointAccountId is null)
				scope.Cancel();
		}
	}

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

	public HostViewModel(
		ITypedNavigator navigator
		, IDesktopIntegrationServiceFactory integrationServiceFactory
		, IHostCredentialProvider credentialProvider
		, IEndpointAccountManager endpointAccountManager
		, IUserInterfaceService userInterfaceService) : base(navigator)
	{
		_navigator = navigator;
		_credentialProvider = credentialProvider;
		_endpointAccountManager = endpointAccountManager;
		_userInterfaceService = userInterfaceService;
		IpcClient = integrationServiceFactory.Create(credentialProvider.Protocol, credentialProvider.Address);
		Title = credentialProvider.Title;
	}
}