using System.Collections.ObjectModel;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.Shared;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Amusoft.PCR.Application.UI.VM;

public partial class CommandButtonListViewModel : PageViewModel
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
}