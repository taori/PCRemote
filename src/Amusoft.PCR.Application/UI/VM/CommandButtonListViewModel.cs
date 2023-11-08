using System.Collections.ObjectModel;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.Shared;
using Amusoft.PCR.Domain.VM;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Amusoft.PCR.Application.UI.VM;

public partial class CommandButtonListViewModel : ReloadablePageViewModel, INavigationCallbacks
{
	[ObservableProperty] 
	private ObservableCollection<NavigationItem> _items = new();

	public CommandButtonListViewModel(ITypedNavigator navigator) : base(navigator)
	{
	}

	protected override string GetDefaultPageTitle()
	{
		return "default";
	}

	public Func<Task<ObservableCollection<NavigationItem>>>? ReloadableItemsProvider { get; set; }

	public Task OnNavigatedAwayAsync()
	{
		ReloadableItemsProvider = null;
		return Task.CompletedTask;
	}

	public Task OnNavigatedToAsync()
	{
		return ReloadAsync();
	}

	protected override async Task OnReloadAsync()
	{
		if (ReloadableItemsProvider is { } provider)
			Items = await provider.Invoke();
	}
}