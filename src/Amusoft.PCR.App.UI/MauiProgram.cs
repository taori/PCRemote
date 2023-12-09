#region

using Amusoft.PCR.Int.UI;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using LogLevel = NLog.LogLevel;

#endregion

namespace Amusoft.PCR.App.UI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var logger = LogManager.Setup()
			.LoadConfigurationFromAssemblyResource(typeof(MauiProgram).Assembly, "Amusoft.PCR.App.UI.Resources.Raw.nlog.config")
			.RegisterMauiLog((_, args) => LogManager.GetLogger("Amusoft.PCR.App.UI.MauiProgram").Fatal(args.ExceptionObject))
			.LoadConfiguration(configurationBuilder =>
			{
				configurationBuilder.ForLogger()
					.FilterMinLevel(LogLevel.Trace)
					.FilterMaxLevel(LogLevel.Fatal)
					.WriteToMauiLogCustom("${message}");
			})
			.GetCurrentClassLogger();

		logger.Debug("Logger configured");

		var builder = MauiApp.CreateBuilder();
		builder.Logging.ClearProviders();
		builder.Logging.AddNLog();

		ServiceRegistrarUI.Register(builder.Services);

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
		
		ResourceBridgeConfiguration.Apply();
		MauiRoutes.Register();

#if DEBUG
		builder.Logging
			.AddDebug()
				.AddFilter(level => level >= Microsoft.Extensions.Logging.LogLevel.Debug);
#endif

		return builder.Build();
	}
}