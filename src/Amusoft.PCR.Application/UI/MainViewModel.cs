using System.Collections.ObjectModel;
using Amusoft.PCR.Domain.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.Application.UI;


public partial class MainViewItemModel : ObservableObject
{
	[ObservableProperty]
	private string? _text;

	[ObservableProperty]
	private string? _imagePath;

	[ObservableProperty]
	private IRelayCommand? _command;
}

public partial class MainViewModel : ObservableObject
{
	private readonly INavigation _navigation;
	private readonly IToast _toast;

	public MainViewModel(INavigation navigation, IToast toast)
	{
		_navigation = navigation;
		_toast = toast;

		_items = new ObservableCollection<MainViewItemModel>()
		{
			new ()
			{
				ImagePath = "configuration.png",
				Text = "Configuration",
				Command = new RelayCommand(() => _navigation.GoToAsync("/PortConfiguration"))
			},
			new ()
			{
				ImagePath = "configuration.png",
				Text = "MainPage",
				Command = new RelayCommand(() => _toast.Make("test").Show())
			}
		};
	}

	[ObservableProperty]
	private ObservableCollection<MainViewItemModel> _items;

	[RelayCommand]
	public async Task GotoPortConfiguration()
	{
		await _navigation.GoToAsync("///PortConfiguration");
	}
}