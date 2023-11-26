using Amusoft.PCR.Domain.VM;
using NLog;

namespace Amusoft.PCR.App.UI;

public partial class AppShell
{
	private static readonly ILogger Log = LogManager.GetLogger(nameof(AppShell));
	
	public AppShell()
	{
		InitializeComponent();
	}

	protected override async void OnNavigated(ShellNavigatedEventArgs args)
	{
		Log.Debug("AppShell.OnNavigated");
		if (Current is { CurrentPage.BindingContext: INavigationCallbacks nav })
			await nav.OnNavigatedToAsync();
	}

	protected override async void OnNavigating(ShellNavigatingEventArgs args)
	{
		Log.Debug("AppShell.OnNavigating");
		if (Current is {CurrentPage.BindingContext: INavigationCallbacks nav })
			await nav.OnNavigatedAwayAsync();
	}
}