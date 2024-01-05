using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using Amusoft.PCR.Domain.UI.Entities;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class HostViewModel : PageViewModel, INavigationCallbacks
{
	private readonly ITypedNavigator _navigator;
	private readonly ILogger<HostViewModel> _logger;
	private readonly IIdentityManagerFactory _identityManagerFactory;
	private readonly ICredentialPrompt _credentialPrompt;
	private readonly IHostCredentials _hostCredentials;
	private readonly IEndpointAccountManager _endpointAccountManager;
	private readonly IUserInterfaceService _userInterfaceService;
	public IIpcIntegrationService IpcClient { get; }

	public Task OnNavigatedToAsync()
	{
		return TryEstablishBearerToken();
	}

	private async Task<bool> TryEstablishBearerToken()
	{
		var endpointAccount = await _endpointAccountManager.TryGetEndpointAccountAsync(_hostCredentials.Address, CancellationToken.None);
		if (endpointAccount is null)
		{
			var userDecision = await _userInterfaceService.DisplayConfirmAsync(Translations.Generic_ActionRequired, Translations.Host_AccountRequiredToInteractWithHost);
			if (userDecision)
				endpointAccount = await _endpointAccountManager.TryGetEndpointAccountAsync(_hostCredentials.Address, CancellationToken.None);
		}

		if (endpointAccount is null)
			return false;

		if (endpointAccount is { BearerTokens.Count: 0 })
		{
			var password = await _credentialPrompt.GetPasswordAsync(_hostCredentials.Title);
			if (password is null)
				return false;

			var identityManager = _identityManagerFactory.Create(_hostCredentials.Address, _hostCredentials.Protocol);
			_logger.LogDebug("Logging in user {Mail}", endpointAccount.Email);
			var login = await identityManager.LoginAsync(endpointAccount.Email, password, CancellationToken.None);
			if (login is null)
			{
				_logger.LogDebug("Login failed. Perhaps we need to register first?");
				if (!await TryRegister(identityManager, endpointAccount, password))
				{
					_logger.LogError("Login failed AND register failed. Unable to continue");
					return false;
				}

				login = await identityManager.LoginAsync(endpointAccount.Email, password, CancellationToken.None);
				if (login is null)
				{
					_logger.LogError("Failed to log in after register. Unable to continue");
					return false;
				}
			}

			await SaveBearerTokenAsync(login, endpointAccount.Id);
		}

		return true;
	}

	private Task SaveBearerTokenAsync(SignInResponse login, Guid endpointAccountId)
	{
		return _endpointAccountManager.AddBearerTokenAsync(new BearerToken(login.AccessToken, login.RefreshToken, login.ValidUntil, DateTimeOffset.Now, endpointAccountId));
	}

	private async Task<bool> TryRegister(IIdentityManager identityManager, EndpointAccount endpointAccount, string password)
	{
		try
		{
			await identityManager.RegisterAsync(endpointAccount.Email, password, CancellationToken.None);
			return true;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to register - Already exists?");
			return false;
		}
	}

	public async Task OnNavigatingAsync(INavigatingContext context)
	{
		if (context.NavigationKind == NavigationKind.Push
		    && !context.TargetLocation.OriginalString.Contains("HostAccounts"))
		{
			await using var scope = context.PauseNavigation();

			if (!await TryEstablishBearerToken())
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
		, ILogger<HostViewModel> logger
		, IIdentityManagerFactory identityManagerFactory
		, ICredentialPrompt credentialPrompt
		, IDesktopIntegrationServiceFactory integrationServiceFactory
		, IHostCredentials hostCredentials
		, IEndpointAccountManager endpointAccountManager
		, IUserInterfaceService userInterfaceService) : base(navigator)
	{
		_navigator = navigator;
		_logger = logger;
		_identityManagerFactory = identityManagerFactory;
		_credentialPrompt = credentialPrompt;
		_hostCredentials = hostCredentials;
		_endpointAccountManager = endpointAccountManager;
		_userInterfaceService = userInterfaceService;
		IpcClient = integrationServiceFactory.Create(hostCredentials.Protocol, hostCredentials.Address);
		Title = hostCredentials.Title;
	}
}