﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Amusoft.PCR.Int.Agent.Windows.Events;
using Amusoft.PCR.Int.Agent.Windows.Interop;
using Amusoft.PCR.Int.Agent.Windows.Services;
using Amusoft.PCR.Int.Agent.Windows.Windows;
using Amusoft.PCR.Int.IPC;
using GrpcDotNetNamedPipes;
using NLog;

namespace Amusoft.PCR.Int.Agent.Windows;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
	private static readonly Logger Log = LogManager.GetLogger(nameof(App));

	private NamedPipeServer? _namedPipeServer;
	private Mutex? _runOnceMutex;

	[Conditional("RELEASE")]
	private void ShutdownIfMutexTaken()
	{
		_runOnceMutex = new Mutex(true, Globals.InteropMutexName, out var mutexNew);
		if (!mutexNew)
		{
			Shutdown();
		}
	}

	[Conditional("RELEASE")]
	private void ReleaseMutex()
	{
		Log.Debug("Releasing mutex");
		_runOnceMutex?.ReleaseMutex();
	}

	protected override void OnStartup(StartupEventArgs e)
	{
		Log.Info("Launching Windows desktop integration");
		base.OnStartup(e);

		Log.Debug("Setting up event handler to check for parent process");
		ProcessExitListenerManager.ProcessExited += ProcessExitListenerManagerOnProcessExited;

		// VerifyMonitorManager();
		// ConfirmSample();
		// VerifySimpleAudioManager();

		TaskScheduler.UnobservedTaskException += (sender, ex) => Log.Fatal(ex.Exception);
		AppDomain.CurrentDomain.UnhandledException += (sender, ex) => Log.Fatal(ex.ExceptionObject);

		ShutdownIfMutexTaken();

		// EventSetup.Initialize();
		// EventSetup.Debug();

		if (!TryLaunchInteropChannel())
		{
			Log.Fatal("Failed to launch named pipe for IPC with web application");
			_namedPipeServer?.Dispose();
		}
	}

	[Conditional("DEBUG")]
	private async void VerifyMonitorManager()
	{
		using var manager = new NativeMonitorManager();
		var brightness = manager.GetAverageBrightness();
		Log.Debug("Average brightness: {Brightness}", brightness);
		Log.Debug("Monitor count: {Count}", manager.Monitors.Count);

		foreach (var monitor in manager.Monitors)
		{
			Log.Debug("Monitor Brightness: {Description} Current: {Current}, Min: {Min}, Max: {Max}", 
				monitor.Description, monitor.CurrentValue, monitor.MinValue, monitor.MaxValue);
		}

		await Task.Delay(2000);
		manager.SetBrightness(50);

		foreach (var monitor in manager.Monitors)
		{
			Log.Debug("Monitor Brightness: {Description} Current: {Current}, Min: {Min}, Max: {Max}",
				monitor.Description, monitor.CurrentValue, monitor.MinValue, monitor.MaxValue);
		}

		await Task.Delay(2000);
		manager.SetBrightness(100);

		foreach (var monitor in manager.Monitors)
		{
			Log.Debug("Monitor Brightness: {Description} Current: {Current}, Min: {Min}, Max: {Max}", 
				monitor.Description, monitor.CurrentValue, monitor.MinValue, monitor.MaxValue);
		}
	}

	private static async void ConfirmSample()
	{
		var request = new GetConfirmRequest("Some title", "Some description");

		var response = await ViewModelSpawner.GetWindowResponseAsync<ConfirmWindow, ConfirmWindowViewModel, GetConfirmRequest, GetConfirmResponse>(request);
	}

	private void ProcessExitListenerManagerOnProcessExited(object? sender, int e)
	{
		Log.Info("Parent process {Id} shut down - exiting program", e);
		Shutdown(0);
	}

	protected override void OnExit(ExitEventArgs e)
	{
		Log.Info("Windows desktop integration shutting down");

		ReleaseMutex();

		Log.Debug("Shutting down named pipe server");
		_namedPipeServer?.Kill();
		_namedPipeServer?.Dispose();
		base.OnExit(e);
	}

	private bool TryLaunchInteropChannel()
	{
		try
		{
			Log.Debug("Initializing NamedPipeServer to listen for service calls");

			_namedPipeServer = new NamedPipeServer(Globals.NamedPipeChannel);
			DesktopIntegrationService.BindService(_namedPipeServer.ServiceBinder, new DesktopIntegrationServiceImplementation());
			VoiceCommandService.BindService(_namedPipeServer.ServiceBinder, new VoiceRecognitionServiceImplementation());

			Log.Debug("Starting IPC Server");
			_namedPipeServer.Start();

			Log.Info("IPC Server is running");
			return true;
		}
		catch (Exception ex)
		{
			Log.Error(ex);
			return false;
		}
	}

#pragma warning disable CS0162
	private void VerifySimpleAudioManager()
	{
		return;
		Task.Run(() =>
		{
			if (!SimpleAudioManager.TryGetAudioFeeds(out var feeds))
			{
				Log.Error("Failed to get audio feeds");
				return;
			}
			else
			{
				foreach (var feed in feeds)
				{
					Log.Debug($"Feed: {feed.Id} {feed.Volume}% name: {feed.Name}");
				}
			}

			Log.Debug("Muted: {Muted}", SimpleAudioManager.GetMasterVolumeMute());
			Thread.Sleep(1000);
			Log.Debug("Inverting Mute: {Muted}", SimpleAudioManager.SetMasterVolumeMute(!SimpleAudioManager.GetMasterVolumeMute() == true));
			Thread.Sleep(1000);
			Log.Debug("Reverting Mute: {Muted}", SimpleAudioManager.SetMasterVolumeMute(!SimpleAudioManager.GetMasterVolumeMute() == true));
			Thread.Sleep(1000);


			var startVolume = SimpleAudioManager.GetMasterVolume();
			Log.Debug("Current volume {Volume}", startVolume);
			Thread.Sleep(1000);
			SimpleAudioManager.SetMasterVolume(60);
			Thread.Sleep(1000);
			Log.Debug("Current volume {Volume}", SimpleAudioManager.GetMasterVolume());
			Thread.Sleep(1000);
			SimpleAudioManager.SetMasterVolume(startVolume ?? 100);
		});
	}
}