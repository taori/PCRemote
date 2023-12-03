using Amusoft.PCR.AM.UI.ViewModels;

namespace Amusoft.PCR.App.UI.Pages;

public partial class Settings : ContentPage
{
	public Settings(SettingsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}