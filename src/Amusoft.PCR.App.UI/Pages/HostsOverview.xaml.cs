using Amusoft.PCR.AM.UI.ViewModels;

namespace Amusoft.PCR.App.UI.Pages;

public partial class HostsOverview : ContentPage
{
	public HostsOverview(HostsOverviewViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}