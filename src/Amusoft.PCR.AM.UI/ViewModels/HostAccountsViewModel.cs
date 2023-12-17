using System.Collections.ObjectModel;
using System.Windows.Input;
using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class HostAccountsViewModel : ReloadablePageViewModel, INavigationCallbacks
{
	private readonly IEndpointRepository _endpointRepository;
	private readonly IHostCredentialProvider _hostCredentials;
	private readonly IEndpointAccountSelection _endpointAccountSelection;

	[ObservableProperty]
	private ObservableCollection<HostAccountViewModel> _items = new();

	public HostAccountsViewModel(
		ITypedNavigator navigator
		, IEndpointRepository endpointRepository
		, IHostCredentialProvider hostCredentials
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
		var selectedAccount = await _endpointAccountSelection.GetCurrentAccountAsync(_hostCredentials.Address);
		var endpoints = await _endpointRepository.GetEndpointAccountsAsync(_hostCredentials.Address);
		Items = new ObservableCollection<HostAccountViewModel>(
			endpoints.Select(d => new HostAccountViewModel(d.Id, d.Email, InteractWithCommand) { Active = d.Id.Equals(selectedAccount) })
		);

		Items.Add(new HostAccountViewModel(Guid.Empty, "test", InteractWithCommand));
	}
}

public partial class HostAccountViewModel : ObservableObject
{
	public HostAccountViewModel(Guid id, string text, ICommand command)
	{
		_id = id;
		_text = text;
		_command = command;
	}

	[ObservableProperty]
	private bool _active;

	[ObservableProperty]
	private Guid _id;

	[ObservableProperty]
	private string _text;

	[ObservableProperty]
	private ICommand _command;
}