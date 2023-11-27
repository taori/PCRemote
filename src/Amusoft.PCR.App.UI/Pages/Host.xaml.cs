using Amusoft.PCR.AM.UI.ViewModels;

namespace Amusoft.PCR.App.UI.Pages;

public partial class Host : ContentPage
{
	public Host(HostViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}