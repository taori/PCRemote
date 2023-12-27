using System.Collections.ObjectModel;
using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using Amusoft.PCR.Domain.Shared.ValueTypes;
using Amusoft.PCR.Domain.UI.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class HostAccountsViewModel : ReloadablePageViewModel, INavigationCallbacks
{
	private readonly ILogger<HostAccountsViewModel> _logger;
	private readonly IIpcIntegrationService _ipcIntegrationService;
	private readonly IUserInterfaceService _userInterfaceService;
	private readonly IEndpointRepository _endpointRepository;
	private readonly IHostCredentials _hostCredentials;
	private readonly IEndpointAccountSelection _endpointAccountSelection;

	[ObservableProperty]
	private ObservableCollection<HostAccountViewModel> _items = new();

	public HostAccountsViewModel(
		ITypedNavigator navigator,
		ILogger<HostAccountsViewModel> logger
		, IIpcIntegrationService ipcIntegrationService
		, IUserInterfaceService userInterfaceService
		, IEndpointRepository endpointRepository
		, IHostCredentials hostCredentials
		, IEndpointAccountSelection endpointAccountSelection) : base(navigator)
	{
		_logger = logger;
		_ipcIntegrationService = ipcIntegrationService;
		_userInterfaceService = userInterfaceService;
		_endpointRepository = endpointRepository;
		_hostCredentials = hostCredentials;
		_endpointAccountSelection = endpointAccountSelection;
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.HostAccounts_Title;
	}

	public Task OnNavigatedToAsync()
	{
		return ReloadAsync();
	}

	[RelayCommand]
	private async Task AddAccount()
	{
		var endpoint = await _endpointRepository.FindEndpointAsync(_hostCredentials.Address, CancellationToken.None);
		if (endpoint is not null)
			await Navigator.OpenHostCreation(endpoint);
	}

	[RelayCommand]
	private async Task SelectAccount(HostAccountViewModel item)
	{
		if (item.Active)
		{
			await _userInterfaceService.DisplayAlertAsync(Translations.Generic_Warning, Translations.HostAccounts_AccountAlreadyActive);
			return;
		}

		var account = await _endpointRepository.GetEndpointAccountAsync(item.Id, CancellationToken.None);
		if (!account.BearerTokens.Any())
		{
			await _userInterfaceService.DisplayAlertAsync(Translations.Generic_Warning, Translations.HostAccounts_ThisAccountCannotBeLoggedIn);
			return;
		}

		await _endpointAccountSelection.SetEndpointAccountAsync(_hostCredentials.Address, item.Id, CancellationToken.None);
	}

	[RelayCommand]
	private async Task RemoveAccount(HostAccountViewModel item)
	{
		using (LoadState.QueueLoading())
		{
			if (item.Active)
			{
				await _userInterfaceService.DisplayAlertAsync(Translations.Generic_Warning, Translations.HostAccounts_AccountCannotBeDeletedWhileActive);
			}

			var account = await _endpointRepository.GetEndpointAccountAsync(item.Id, CancellationToken.None);

			if (await _ipcIntegrationService.IdentityExtendedClient.TryDeleteUserAsync(account.Email, CancellationToken.None) == true)
			{
				await _endpointRepository.RemoveEndpointAccountAsync(item.Id);
				await ReloadAsync();
			}
		}
	}

	[RelayCommand]
	private Task ResetPassword(HostAccountViewModel item)
	{
		return Task.CompletedTask;
	}

	[RelayCommand]
	private async Task ToggleAdminAccount(HostAccountViewModel item)
	{
		var endpoint = await _endpointRepository.GetEndpointAccountAsync(item.Id, CancellationToken.None);
		var isAdmin = await _ipcIntegrationService.IdentityExtendedClient.RequestAdminToggle(endpoint.Email, CancellationToken.None);

		if (Items.FirstOrDefault(d => d.Id == item.Id) is { } match)
		{
			match.IsAdmin = isAdmin == true;
		}
	}

	[RelayCommand]
	private async Task ChangePermissions(HostAccountViewModel item)
	{
		var endpoint = await _endpointRepository.GetEndpointAccountAsync(item.Id, CancellationToken.None);
		await Navigator.OpenEndpointAccountPermissions(endpoint);
	}

	protected override async Task OnReloadAsync(CancellationToken cancellationToken)
	{
		var selectedAccount = await _endpointAccountSelection.GetCurrentAccountAsync(_hostCredentials.Address, cancellationToken);
		await UpdateEndpointsFromRemoteServerAsync(cancellationToken);
		var endpoints = await _endpointRepository.GetEndpointAccountsAsync(_hostCredentials.Address, cancellationToken);
		var adminStates = new Dictionary<string, bool?>();
		foreach (var email in endpoints.Select(d => d.Email))
		{
			var permissions = await _ipcIntegrationService.IdentityExtendedClient.GetPermissionsAsync(email, CancellationToken.None);
			if (permissions is not null)
				adminStates.Add(email, permissions.UserType == UserType.Administrator);
		}

		var viewModels = endpoints.Select(d => new HostAccountViewModel(d) { Active = d.Id.Equals(selectedAccount), IsAdmin = adminStates.TryGetValue(d.Email, out var admin) && admin!.Value });
		Items = new ObservableCollection<HostAccountViewModel>(
			viewModels.OrderByDescending(d => d.Active)
		);
	}

	private async Task UpdateEndpointsFromRemoteServerAsync(CancellationToken cancellationToken)
	{
		var endpoint = await _endpointRepository.FindEndpointAsync(_hostCredentials.Address, cancellationToken);
		if (endpoint is null)
		{
			_logger.LogError("Unable to retrieve endpoint for {Address}", _hostCredentials.Address);
			return;
		}

		var remoteUsers = await _ipcIntegrationService.IdentityExtendedClient.GetRegisteredUsersAsync(cancellationToken);
		if (remoteUsers is null)
		{
			_logger.LogError("Failed to obtain users from remote source. Aborting update");
			return;
		}

		var localUsers = await _endpointRepository.GetEndpointAccountsAsync(_hostCredentials.Address, cancellationToken);
		var localEmails = new HashSet<string>(localUsers.Select(d => d.Email), StringComparer.OrdinalIgnoreCase);
		foreach (var remoteUser in remoteUsers)
		{
			if (!localEmails.Contains(remoteUser.Email))
			{
				_logger.LogInformation("Creating endpoint from remote registration for {Address}", remoteUser.Email);
				await _endpointRepository.CreateEndpointAccountAsync(endpoint.Id, remoteUser.Email, cancellationToken);
			}
		}
	}
}

public partial class HostAccountViewModel : ObservableObject
{
	private readonly EndpointAccount _account;

	[ObservableProperty]
	private bool _active;

	[ObservableProperty]
	private Guid _id;

	[ObservableProperty]
	private string _text;

	[ObservableProperty]
	private bool _isAdmin;

	[ObservableProperty]
	private DateTimeOffset? _lastUsed;

	public HostAccountViewModel(EndpointAccount account)
	{
		_account = account;
		_id = account.Id;
		_text = account.Email;
		_lastUsed = account.BearerTokens.OrderByDescending(d => d.IssuedAt).Select(d => d.IssuedAt).FirstOrDefault();
	}
}