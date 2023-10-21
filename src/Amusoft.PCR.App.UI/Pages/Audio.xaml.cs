using Amusoft.PCR.Application.UI;
using Amusoft.PCR.Application.UI.VM;

namespace Amusoft.PCR.App.UI.Pages;

public partial class Audio : ContentPage
{
	public Audio(AudioViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}