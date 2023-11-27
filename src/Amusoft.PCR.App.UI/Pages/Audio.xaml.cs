using Amusoft.PCR.AM.UI.ViewModels;

namespace Amusoft.PCR.App.UI.Pages;

public partial class Audio : ContentPage
{
	public Audio(AudioViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}