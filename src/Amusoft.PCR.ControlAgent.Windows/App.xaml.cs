using Amusoft.PCR.ControlAgent.Shared;
using Amusoft.PCR.ControlAgent.Windows.Events;
using Amusoft.PCR.ControlAgent.Windows.Interop;
using Amusoft.PCR.ControlAgent.Windows.Services;
using NLog;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GrpcDotNetNamedPipes;
using DesktopIntegrationService = Amusoft.PCR.ControlAgent.Shared.DesktopIntegrationService;

namespace Amusoft.PCR.ControlAgent.Windows;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
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

		ShutdownIfMutexTaken();

		// EventSetup.Initialize();
		// EventSetup.Debug();

		VerifySimpleAudioManager();
		if (!TryLaunchInteropChannel())
		{
			Log.Fatal("Failed to launch named pipe for IPC with web application");
			_namedPipeServer?.Dispose();
		}
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

		_namedPipeServer = new NamedPipeServer(Globals.NamedPipeChannel);
		DesktopIntegrationService.BindService(_namedPipeServer.ServiceBinder, new DesktopIntegrationServiceImplementation());
		VoiceCommandService.BindService(_namedPipeServer.ServiceBinder, new VoiceRecognitionServiceImplementation());

		try
		{
			Log.Info("Starting IPC");
			_namedPipeServer.Start();

			Log.Info("IPC running");
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
			Log.Debug("Inverting Mute: {Muted}", SimpleAudioManager.SetMasterVolumeMute(!SimpleAudioManager.GetMasterVolumeMute()));
			Thread.Sleep(1000);
			Log.Debug("Reverting Mute: {Muted}", SimpleAudioManager.SetMasterVolumeMute(!SimpleAudioManager.GetMasterVolumeMute()));
			Thread.Sleep(1000);


			var startVolume = SimpleAudioManager.GetMasterVolume();
			Log.Debug("Current volume {Volume}", startVolume);
			Thread.Sleep(1000);
			SimpleAudioManager.SetMasterVolume(60);
			Thread.Sleep(1000);
			Log.Debug("Current volume {Volume}", SimpleAudioManager.GetMasterVolume());
			Thread.Sleep(1000);
			SimpleAudioManager.SetMasterVolume(startVolume);
		});
	}
}