using System.Collections.ObjectModel;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Amusoft.PCR.AM.UI.ViewModels;

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

	protected override async Task OnReloadAsync(CancellationToken cancellationToken)
	{
		if (ReloadableItemsProvider is { } provider)
			Items = await provider.Invoke();
	}
}