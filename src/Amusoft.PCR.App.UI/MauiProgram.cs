using Amusoft.PCR.App.UI.Controls;
using Amusoft.PCR.App.UI.Pages;
using Amusoft.PCR.Application.Resources;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.App.UI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		MauiServiceRegistrar.Register(builder.Services);

		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureMauiHandlers(handlers =>
			{
				handlers.AddHandler<TrackerView, TrackerViewHandler>();
			})
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		
		MauiRoutes.Register();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}