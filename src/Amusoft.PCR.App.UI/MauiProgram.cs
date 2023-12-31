﻿using System.Diagnostics;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Int.UI;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using LogLevel = NLog.LogLevel;

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
				configurationBuilder
					.ForLogger()
					.FilterMinLevel(LogLevel.Debug)
					.WriteToMauiLogCustom("${message}");
				configurationBuilder
					.ForLogger()
					.FilterMinLevel(LogLevel.Error)
					.WriteTo(new ToastTarget());
			})
			.GetCurrentClassLogger();

		RunApplicationStartup();

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
				fonts.AddFont("OpenSans-Regular.ttf", FontNames.OpenSansRegular);
				fonts.AddFont("OpenSans-Semibold.ttf", FontNames.OpenSansSemibold);
				fonts.AddFont("MaterialIcons-Regular.ttf", FontNames.MaterialIcons);
				fonts.AddFont("MaterialIconsOutlined-Regular.otf", FontNames.MaterialIconsOutlined);
				fonts.AddFont("MaterialIconsRound-Regular.otf", FontNames.MaterialIconsRound);
				fonts.AddFont("MaterialIconsSharp-Regular.otf", FontNames.MaterialIconsSharp);
				fonts.AddFont("MaterialIconsTwoTone-Regular.otf", FontNames.MaterialIconsTwoTone);
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

	private static void RunApplicationStartup()
	{
		try
		{
			// var after = Directory.EnumerateFiles(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "*.db", SearchOption.AllDirectories)
			// 	.ToArray();
			var serviceCollection = new ServiceCollection();
			ServiceRegistrarUI.Register(serviceCollection);
			var serviceProvider = serviceCollection.BuildServiceProvider();
			var startupInstances = serviceProvider.GetServices<IApplicationStartup>();
			foreach (var startupInstance in startupInstances)
			{
				startupInstance.Apply();
			}
		}
		catch (Exception e)
		{
			Debug.WriteLine(e.ToString());
		}
	}
}