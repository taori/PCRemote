using Amusoft.PCR.Domain.VM;

namespace Amusoft.PCR.App.UI;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
	}

	protected override async void OnNavigated(ShellNavigatedEventArgs args)
	{
		if (Current is { CurrentPage.BindingContext: INavigationCallbacks nav })
			await nav.OnNavigatedToAsync();
	}

	protected override async void OnNavigating(ShellNavigatingEventArgs args)
	{
		if (Current is {CurrentPage.BindingContext: INavigationCallbacks nav })
			await nav.OnNavigatedAwayAsync();
	}
}