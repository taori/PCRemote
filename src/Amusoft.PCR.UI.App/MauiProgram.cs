using Amusoft.PCR.UI.App.Pages;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.UI.App;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		Routing.RegisterRoute(nameof(PortConfiguration), typeof(PortConfiguration));
		Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}