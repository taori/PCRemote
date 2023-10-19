
using Amusoft.PCR.Application.Features.DesktopIntegration;

namespace Amusoft.PCR.App.Service.Services;

public class DesktopIntegrationLauncherServiceDelegate : BackgroundService
{
	private readonly DesktopIntegrationLauncherService _launcherService;

	public DesktopIntegrationLauncherServiceDelegate(DesktopIntegrationLauncherService launcherService)
	{
		_launcherService = launcherService;
	}

	public override void Dispose()
	{
		_launcherService.Dispose();
		base.Dispose();
	}

	public override async Task StartAsync(CancellationToken cancellationToken)
	{
		await _launcherService.StartAsync(cancellationToken).ConfigureAwait(false);
		await base.StartAsync(cancellationToken);
	}

	public override async Task StopAsync(CancellationToken cancellationToken)
	{
		await _launcherService.StopAsync(cancellationToken).ConfigureAwait(false);
		await base.StopAsync(cancellationToken);
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await _launcherService.ExecuteAsync(stoppingToken).ConfigureAwait(false);
	}
}