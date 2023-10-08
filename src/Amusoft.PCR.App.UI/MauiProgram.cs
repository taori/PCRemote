using Amusoft.PCR.App.UI.Pages;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.App.UI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		ServiceRegistrar.Register(builder.Services);

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