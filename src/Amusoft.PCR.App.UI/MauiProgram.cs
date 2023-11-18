﻿using Amusoft.PCR.App.UI.Controls;
using Amusoft.PCR.App.UI.Pages;
using Amusoft.PCR.Application.Resources;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Amusoft.PCR.App.UI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var logger = LogManager.Setup()
			.LoadConfigurationFromFile("nlog.config")
			.RegisterMauiLog((_, args) => LogManager.GetLogger("Application").Fatal(args.ExceptionObject))
			.GetCurrentClassLogger();

		logger.Debug("Logger configured");

		var builder = MauiApp.CreateBuilder();
		builder.Logging.ClearProviders();
		builder.Logging.AddNLog();

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
		builder.Logging
			.AddDebug()
				.AddFilter(level => level >= LogLevel.Debug);
#endif

		return builder.Build();
	}
}