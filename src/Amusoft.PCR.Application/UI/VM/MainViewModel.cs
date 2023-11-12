using System.Collections.ObjectModel;
using Amusoft.PCR.Application.Resources;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.Shared;
using Amusoft.PCR.Domain.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.Application.UI.VM;

public partial class MainViewModel : PageViewModel
{
	public MainViewModel(ITypedNavigator navigator) : base(navigator)
	{
		_items = new ObservableCollection<NavigationItem>()
		{
			new ()
			{
				ImagePath = null,
				Text = Translations.Page_Title_HostsOverview,
				Command = new RelayCommand(() => Navigator.OpenHostOverview())
			},
			new ()
			{
				ImagePath = null,
				Text = Translations.Page_Title_Settings,
				Command = new RelayCommand(() => Navigator.OpenSettings())
			},
			new ()
			{
				ImagePath = null,
				Text = "Logs",
				Command = new RelayCommand(() => Navigator.OpenLogs())
			},
		};
	}

	[ObservableProperty]
	private ObservableCollection<NavigationItem> _items;

	protected override string GetDefaultPageTitle()
	{
		return Translations.Page_Title_MainPage;
	}
}