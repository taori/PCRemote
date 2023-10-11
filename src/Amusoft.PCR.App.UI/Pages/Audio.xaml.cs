using Amusoft.PCR.Application.UI;

namespace Amusoft.PCR.App.UI.Pages;

public partial class Audio : ContentPage
{
	public Audio(AudioViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}