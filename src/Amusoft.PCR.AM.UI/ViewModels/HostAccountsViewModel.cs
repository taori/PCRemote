using System.Collections.ObjectModel;
using System.Windows.Input;
using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using Amusoft.PCR.Domain.UI.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class HostAccountsViewModel : ReloadablePageViewModel, INavigationCallbacks
{
	private readonly IEndpointRepository _endpointRepository;
	private readonly IHostCredentials _hostCredentials;
	private readonly IEndpointAccountSelection _endpointAccountSelection;

	[ObservableProperty]
	private ObservableCollection<HostAccountViewModel> _items = new();

	public HostAccountsViewModel(
		ITypedNavigator navigator
		, IEndpointRepository endpointRepository
		, IHostCredentials hostCredentials
		, IEndpointAccountSelection endpointAccountSelection) : base(navigator)
	{
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
	private Task InteractWith(HostAccountViewModel item)
	{
		return Task.CompletedTask;
	}

	protected override async Task OnReloadAsync(CancellationToken cancellationToken)
	{
		var selectedAccount = await _endpointAccountSelection.GetCurrentAccountAsync(_hostCredentials.Address, cancellationToken);
		var endpoints = await _endpointRepository.GetEndpointAccountsAsync(_hostCredentials.Address, cancellationToken);
		Items = new ObservableCollection<HostAccountViewModel>(
			endpoints.Select(d => new HostAccountViewModel(d, InteractWithCommand) { Active = d.Id.Equals(selectedAccount) })
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
	private ICommand _command;

	public HostAccountViewModel(EndpointAccount account, ICommand command)
	{
		_account = account;
		_command = command;
		_id = account.Id;
		_text = account.Email;
	}
}