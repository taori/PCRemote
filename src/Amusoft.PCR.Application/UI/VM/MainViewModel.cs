using System.Collections.ObjectModel;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Translations = Amusoft.PCR.AM.Shared.Resources.Translations;

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
				Text = AM.Shared.Resources.Translations.Page_Title_HostsOverview,
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
#if DEBUG
			new ()
			{
				ImagePath = null,
				Text = "Debug",
				Command = new RelayCommand(() => Navigator.OpenDebug())
			},
#endif
		};
	}

	[ObservableProperty]
	private ObservableCollection<NavigationItem> _items;

	protected override string GetDefaultPageTitle()
	{
		return AM.Shared.Resources.Translations.Page_Title_MainPage;
	}
}