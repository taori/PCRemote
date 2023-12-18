using System.Collections.ObjectModel;
using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using Amusoft.PCR.Domain.UI.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class HostAccountsViewModel : ReloadablePageViewModel, INavigationCallbacks
{
	private readonly IUserInterfaceService _userInterfaceService;
	private readonly IEndpointRepository _endpointRepository;
	private readonly IHostCredentials _hostCredentials;
	private readonly IEndpointAccountSelection _endpointAccountSelection;

	[ObservableProperty]
	private ObservableCollection<HostAccountViewModel> _items = new();

	public HostAccountsViewModel(
		ITypedNavigator navigator
		, IUserInterfaceService userInterfaceService
		, IEndpointRepository endpointRepository
		, IHostCredentials hostCredentials
		, IEndpointAccountSelection endpointAccountSelection) : base(navigator)
	{
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
	private Task ChangePermissions(HostAccountViewModel item)
	{
		return Task.CompletedTask;
	}

	protected override async Task OnReloadAsync(CancellationToken cancellationToken)
	{
		var selectedAccount = await _endpointAccountSelection.GetCurrentAccountAsync(_hostCredentials.Address, cancellationToken);
		var endpoints = await _endpointRepository.GetEndpointAccountsAsync(_hostCredentials.Address, cancellationToken);
		Items = new ObservableCollection<HostAccountViewModel>(
			endpoints.Select(d => new HostAccountViewModel(d) { Active = d.Id.Equals(selectedAccount) })
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
	private DateTimeOffset? _lastUsed;

	public HostAccountViewModel(EndpointAccount account)
	{
		_account = account;
		_id = account.Id;
		_text = account.Email;
		_lastUsed = account.BearerTokens.OrderByDescending(d => d.IssuedAt).Select(d => d.IssuedAt).FirstOrDefault();
	}
}