using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.App.UI.Dependencies;
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
		{
			try
			{
				await nav.OnNavigatedToAsync();
			}
			catch (Exception e)
			{
				Log.Error(e, "Error while calling OnNavigated");
			}
		}
	}

	protected override async void OnNavigating(ShellNavigatingEventArgs args)
	{
		Log.Trace("AppShell.OnNavigating");
		if (Current is { CurrentPage.BindingContext: INavigationCallbacks nav })
		{
			try
			{
				await nav.OnNavigatingAsync(new NavigatingScope(args));
			}
			catch (Exception e)
			{
				Log.Error(e, "Error while calling OnNavigating");
			}
		}
	}
}