using System.Collections.ObjectModel;
using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using Amusoft.PCR.Domain.Shared.ValueTypes;
using Amusoft.PCR.Domain.UI.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class HostAccountsViewModel : ReloadablePageViewModel, INavigationCallbacks
{
	private readonly IIpcIntegrationService _ipcIntegrationService;
	private readonly IUserInterfaceService _userInterfaceService;
	private readonly IEndpointRepository _endpointRepository;
	private readonly IHostCredentials _hostCredentials;
	private readonly IEndpointAccountSelection _endpointAccountSelection;

	[ObservableProperty]
	private ObservableCollection<HostAccountViewModel> _items = new();

	public HostAccountsViewModel(
		ITypedNavigator navigator
		, IIpcIntegrationService ipcIntegrationService
		, IUserInterfaceService userInterfaceService
		, IEndpointRepository endpointRepository
		, IHostCredentials hostCredentials
		, IEndpointAccountSelection endpointAccountSelection) : base(navigator)
	{
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
	private Task AddAccount()
	{
		return Task.CompletedTask;
	}

	[RelayCommand]
	private Task SelectAccount(HostAccountViewModel item)
	{
		if (item.Active)
		{
			return _userInterfaceService.DisplayAlertAsync(Translations.Generic_Warning, Translations.HostAccounts_AccountAlreadyActive);
		}

		return _endpointAccountSelection.SetEndpointAccountAsync(_hostCredentials.Address, item.Id, CancellationToken.None);
	}

	[RelayCommand]
	private Task RemoveAccount(HostAccountViewModel item)
	{
		if (item.Active)
		{
			return _userInterfaceService.DisplayAlertAsync(Translations.Generic_Warning, Translations.HostAccounts_AccountCannotBeDeletedWhileActive);
		}

		return _endpointRepository.RemoveEndpointAccountAsync(item.Id);
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
		var permissions = await _ipcIntegrationService.IdentityExtendedClient.GetPermissionsAsync(endpoint.Email, CancellationToken.None);
	}

	protected override async Task OnReloadAsync(CancellationToken cancellationToken)
	{
		var selectedAccount = await _endpointAccountSelection.GetCurrentAccountAsync(_hostCredentials.Address, cancellationToken);
		var endpoints = await _endpointRepository.GetEndpointAccountsAsync(_hostCredentials.Address, cancellationToken);
		var adminStates = new Dictionary<string, bool?>();
		foreach (var email in endpoints.Select(d => d.Email))
		{
			var permissions = await _ipcIntegrationService.IdentityExtendedClient.GetPermissionsAsync(email, CancellationToken.None);
			if (permissions is not null)
				adminStates.Add(email, permissions.UserType == UserType.Administrator);
		}
		Items = new ObservableCollection<HostAccountViewModel>(
			endpoints.Select(d => new HostAccountViewModel(d) { Active = d.Id.Equals(selectedAccount), IsAdmin = adminStates.TryGetValue(d.Email, out var admin) && admin!.Value })
		);
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