using Amusoft.PCR.Application.UI.VM;

namespace Amusoft.PCR.App.UI.Pages;

public partial class Host : ContentPage
{
	public Host(HostViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}