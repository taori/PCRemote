using System.Collections.ObjectModel;
using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.Shared.Utility;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class MainViewModel : PageViewModel, INavigationCallbacks
{
	private readonly IEnumerable<IMainInitializer> _initializers;

	public LoadState LoadState { get; set; } = new();

	public MainViewModel(ITypedNavigator navigator, IEnumerable<IMainInitializer> initializers) : base(navigator)
	{
		_initializers = initializers;
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

	private bool _initializersExecuted;

	public async Task OnNavigatedToAsync()
	{
		using (LoadState.QueueLoading())
		{
			if (!_initializersExecuted && _initializers is { } initalizers && initalizers.Any())
			{
				_initializersExecuted = true;
				await Task.WhenAll(initalizers.Select(d => d.ApplyAsync()));
			}
		}
	}

	[ObservableProperty]
	private ObservableCollection<NavigationItem> _items;

	protected override string GetDefaultPageTitle()
	{
		return Translations.Page_Title_MainPage;
	}
}