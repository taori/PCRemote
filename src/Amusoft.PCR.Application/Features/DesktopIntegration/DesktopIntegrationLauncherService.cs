using Amusoft.PCR.Domain.Services;
using Amusoft.Toolkit.Impersonation;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Amusoft.PCR.Application.Services;

namespace Amusoft.PCR.Application.Features.DesktopIntegration;

public class DesktopIntegrationLauncherService : IBackgroundService
{
	private readonly ILogger<DesktopIntegrationLauncherService> _logger;
	private readonly IIntegrationApplicationLocator _integrationApplicationLocator;
	private readonly IApplicationStateTransmitter _applicationStateTransmitter;
	private readonly IDesktopClientMethods _desktopClient;
	private bool _canOperate;

	public DesktopIntegrationLauncherService(ILogger<DesktopIntegrationLauncherService> logger, IIntegrationApplicationLocator integrationApplicationLocator, IApplicationStateTransmitter applicationStateTransmitter, IDesktopClientMethods desktopClient)
	{
		_logger = logger;
		_integrationApplicationLocator = integrationApplicationLocator;
		_applicationStateTransmitter = applicationStateTransmitter;
		_desktopClient = desktopClient;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogDebug("{Name} starting", nameof(DesktopIntegrationLauncherService));
		_canOperate = _integrationApplicationLocator.IsOperational();
		if (_canOperate)
		{
			_logger.LogInformation("{Name} is working", nameof(DesktopIntegrationLauncherService));
		}
		else
		{
			_logger.LogCritical("{Name} is NOT operational", nameof(DesktopIntegrationLauncherService));
		}

		return Task.CompletedTask;
	}

	public async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var waitDuration = TimeSpan.FromSeconds(60);
		_logger.LogDebug("Checking for integration up state every {Seconds} seconds", waitDuration.TotalSeconds);
		while (!stoppingToken.IsCancellationRequested && _canOperate)
		{
			if (!_integrationApplicationLocator.IsRunning())
			{
				await TryLaunchIntegrationAsync();
				await Task.Delay(2000, stoppingToken);

				var suicideConfirm = await _desktopClient.SuicideOnProcessExit(Process.GetCurrentProcess().Id);
				if (suicideConfirm != true)
				{
					_logger.LogWarning("Integration process is unable to terminate itself, because it failed to confirm parentship.");
				}
			}

			_logger.LogTrace("Waiting for next turn to check if integration backend is working ({Time}ms)", waitDuration.TotalMilliseconds);
			await Task.Delay(waitDuration, stoppingToken);
		}
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		if (_canOperate)
		{
			_logger.LogInformation("Terminating current integration instances");

			var processIds = _integrationApplicationLocator.GetRunningProcessIds().ToArray();
			if (processIds.Length > 0)
			{
				_logger.LogDebug("Terminating {Count} instances", processIds.Length);
				foreach (var processId in processIds)
				{
					_logger.LogDebug("Killing process {Id}", processId);
					Process.GetProcessById(processId).Kill();
				}
			}
			else
			{
				_logger.LogWarning("No integration instances found. It must have crashed?");
			}
		}
		else
		{
			_logger.LogInformation("Not operational. Nothing to do");
		}

		return Task.CompletedTask;
	}

	private async Task<bool> TryLaunchIntegrationAsync()
	{
		try
		{
			_logger.LogDebug("Waiting for application configuration to be done");
			await _applicationStateTransmitter.ConfigurationDone;

			var fullPath = _integrationApplicationLocator.GetAbsolutePath();
			if (!File.Exists(fullPath))
			{
				_logger.LogCritical("Cannot launch {Path} because it does not exist", fullPath);
				return false;
			}

			_logger.LogInformation("Launching application at {Path}", fullPath);

			ProcessImpersonation.Launch(fullPath);

			return true;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling {Name}", nameof(TryLaunchIntegrationAsync));
			return false;
		}
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
	}
}