using Amusoft.PCR.Application.UI.VM;

namespace Amusoft.PCR.App.UI.Pages;

public partial class Settings : ContentPage
{
	public Settings(SettingsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}