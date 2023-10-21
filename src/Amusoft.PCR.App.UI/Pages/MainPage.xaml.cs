using Amusoft.PCR.Application.UI;
using Amusoft.PCR.Application.UI.VM;

namespace Amusoft.PCR.App.UI.Pages;

public partial class MainPage : ContentPage
{
	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}