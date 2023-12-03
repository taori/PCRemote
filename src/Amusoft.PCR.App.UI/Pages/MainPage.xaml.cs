using Amusoft.PCR.AM.UI.ViewModels;

namespace Amusoft.PCR.App.UI.Pages;

public partial class MainPage : ContentPage
{
	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}