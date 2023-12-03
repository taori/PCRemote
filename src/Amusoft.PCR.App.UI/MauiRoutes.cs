using Amusoft.PCR.AM.UI.Resources;
using Amusoft.PCR.App.UI.Pages;

namespace Amusoft.PCR.App.UI;

public static class MauiRoutes
{
	public static void Register()
	{
		Routing.RegisterRoute(PageNames.Host, typeof(Host));
		Routing.RegisterRoute(PageNames.HostsOverview, typeof(HostsOverview));
		Routing.RegisterRoute(PageNames.Settings, typeof(Settings));
		Routing.RegisterRoute(PageNames.Audio, typeof(Audio));
	}
}