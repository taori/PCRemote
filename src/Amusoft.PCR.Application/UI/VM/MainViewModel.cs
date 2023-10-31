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
	private readonly INavigation _navigation;

	public MainViewModel(INavigation navigation, ITypedNavigator navigator) : base(navigator)
	{
		_navigation = navigation;

		_items = new ObservableCollection<NavigationItem>()
		{
			new ()
			{
				ImagePath = "configuration.png",
				Text = Translations.Page_Title_HostsOverview,
				Command = new RelayCommand(() => Navigator.OpenHostOverview())
			},
			new ()
			{
				ImagePath = "configuration.png",
				Text = Translations.Page_Title_Settings,
				Command = new RelayCommand(() => _navigation.GoToAsync($"/{PageNames.Settings}"))
			},
			new ()
			{
				ImagePath = "configuration.png",
				Text = Translations.Page_Title_Audio,
				Command = new RelayCommand(() => _navigation.GoToAsync($"/{PageNames.Audio}"))
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