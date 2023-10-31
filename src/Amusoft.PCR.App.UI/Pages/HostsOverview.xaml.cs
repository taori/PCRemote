using Amusoft.PCR.Application.UI.VM;

namespace Amusoft.PCR.App.UI.Pages;

public partial class HostsOverview : ContentPage
{
	public HostsOverview(HostsOverviewViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}