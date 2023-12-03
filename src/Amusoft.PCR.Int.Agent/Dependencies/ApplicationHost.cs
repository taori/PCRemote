using System.Diagnostics;
using Amusoft.PCR.AM.Agent.Interfaces;
using Amusoft.PCR.Int.Agent.Resources;
using Amusoft.PCR.Int.Agent.Services;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.Agent.Dependencies;

public class ApplicationHost : IApplicationHost, IDisposable
{
	private readonly ILogger<ApplicationHost> _logger;
	private readonly IApplicationController _applicationController;
	private readonly IInterprocessCommunicationServer _interprocessCommunicationServer;
	private Mutex? _runOnceMutex;

	public ApplicationHost(ILogger<ApplicationHost> logger, IApplicationController applicationController, IInterprocessCommunicationServer interprocessCommunicationServer)
	{
		_logger = logger;
		_applicationController = applicationController;
		_interprocessCommunicationServer = interprocessCommunicationServer;
	}
	
	[Conditional("RELEASE")]
	private void ShutdownIfMutexTaken()
	{
		_runOnceMutex = new Mutex(true, Globals.InteropMutexName, out var mutexNew);
		if (!mutexNew)
		{
			_logger.LogWarning("Shutting down {ProcessId} because the mutex was taken", Environment.ProcessId);
			_applicationController.Shutdown();
		}
	}

	[Conditional("RELEASE")]
	private void ReleaseMutex()
	{
		_logger.LogDebug("Releasing mutex");
		_runOnceMutex?.ReleaseMutex();
	}
	
	public void ExecuteStartup()
	{
		_logger.LogDebug("Application is starting up");
		ShutdownIfMutexTaken();
		
		_logger.LogTrace("Setting up event handler to check for parent process");
		ProcessExitListenerManager.ProcessExited += ProcessExitListenerManagerOnProcessExited;

		_logger.LogTrace("Setting up error listeners");
		TaskScheduler.UnobservedTaskException += (sender, ex) => _logger.LogCritical(ex.Exception, "TaskScheduler.UnobservedTaskException");
		AppDomain.CurrentDomain.UnhandledException += (sender, ex) => _logger.LogCritical(ex.ExceptionObject.ToString(), "AppDomain.CurrentDomain.UnhandledException");

		_interprocessCommunicationServer.Start();
	}

	private void ProcessExitListenerManagerOnProcessExited(object? sender, int e)
	{
		_logger.LogInformation("Parent process {Id} shut down - exiting program", e);
		_applicationController.Shutdown();
	}

	public void ExecuteShutdown()
	{
		_logger.LogDebug("Application is shutting down");
		_interprocessCommunicationServer.Stop();
		ReleaseMutex();
	}

	public void Dispose()
	{
		_runOnceMutex?.Dispose();
	}
}