using Amusoft.PCR.Domain.VM;
using NLog;

namespace Amusoft.PCR.App.UI;

public partial class AppShell
{
	private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
	
	public AppShell()
	{
		InitializeComponent();
	}

	protected override async void OnNavigated(ShellNavigatedEventArgs args)
	{
		Log.Trace("AppShell.OnNavigated");
		if (Current is { CurrentPage.BindingContext: INavigationCallbacks nav })
			await nav.OnNavigatedToAsync();
	}

	protected override async void OnNavigating(ShellNavigatingEventArgs args)
	{
		Log.Trace("AppShell.OnNavigating");
		if (Current is {CurrentPage.BindingContext: INavigationCallbacks nav })
			await nav.OnNavigatedAwayAsync();
	}
}