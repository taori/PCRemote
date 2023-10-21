using System.Collections.ObjectModel;
using Amusoft.PCR.Application.Resources;
using Amusoft.PCR.Application.UI.Repos;
using Amusoft.PCR.Domain.VM;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.Application.UI.VM;

public partial class HostsOverviewViewModel : Shared.ReloadablePageViewModel, INavigationCallbacks
{
	private readonly HostRepository _hostRepository;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsErrorLabelVisible))]
	private ObservableCollection<HostItemViewModel> _items = new();

	public bool IsErrorLabelVisible
	{
		get => Items?.Count == 0;
	}

	public HostsOverviewViewModel(HostRepository hostRepository)
	{
		_hostRepository = hostRepository;
	}

	protected override async Task OnReloadAsync()
	{
		var ports = await _hostRepository.GetHostPortsAsync();
		Items = new ObservableCollection<HostItemViewModel>(ports.Select(d => new HostItemViewModel(d.ToString())));
	}

	[RelayCommand]
	public Task ConfigureHostsAsync()
	{
		Items.Add(new HostItemViewModel("Test"));
		return Task.CompletedTask;
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.Page_Title_HostsOverview;
	}

	public Task OnNavigatedToAsync()
	{
		return OnReloadAsync();
	}
}

public partial class HostItemViewModel : ObservableObject
{
	[ObservableProperty]
	private string _name;

	public HostItemViewModel(string name)
	{
		Name = name;
	}
}