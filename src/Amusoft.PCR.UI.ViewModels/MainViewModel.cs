using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Amusoft.PCR.UI.ViewModels;

public partial class MainViewItemModel : ObservableObject
{
	[ObservableProperty]
	private string _text;

	[ObservableProperty]
	private string _imagePath;

	[ObservableProperty]
	private IRelayCommand _command;
}

public partial class MainViewModel : ObservableObject
{
	public MainViewModel()
	{
		_items = new ObservableCollection<MainViewItemModel>()
		{
			new ()
			{
				ImagePath = "configuration.png", 
				Text = "Configuration", 
				Command = new RelayCommand(() => Shell.Current.GoToAsync("/PortConfiguration"))
			},
			new ()
			{
				ImagePath = "configuration.png", 
				Text = "MainPage", 
				Command = new RelayCommand(() => Toast.Make("test", ToastDuration.Long).Show())
			}
		};
	}

	[ObservableProperty]
	private ObservableCollection<MainViewItemModel> _items;

	[RelayCommand]
	public async Task GotoPortConfiguration()
	{
		await Shell.Current.GoToAsync("///PortConfiguration");
	}
}