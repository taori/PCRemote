using System.Collections.ObjectModel;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.Shared;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Amusoft.PCR.Application.UI.VM;

public partial class StaticCommandButtonListViewModel : PageViewModel
{
	[ObservableProperty] 
	private ObservableCollection<NavigationItem> _items = new();

	public StaticCommandButtonListViewModel(ITypedNavigator navigator) : base(navigator)
	{
	}

	protected override string GetDefaultPageTitle()
	{
		return "default";
	}
}