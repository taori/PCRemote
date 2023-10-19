using Amusoft.PCR.Application.Features.DesktopIntegration;

namespace Amusoft.PCR.App.Service.Services;

public class ClientDiscoveryDelegate : BackgroundService
{
	private readonly ClientDiscoveryService _clientDiscoveryService;

	public ClientDiscoveryDelegate(ClientDiscoveryService clientDiscoveryService)
	{
		_clientDiscoveryService = clientDiscoveryService;
	}

	public override void Dispose()
	{
		_clientDiscoveryService.Dispose();
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await _clientDiscoveryService.ExecuteAsync(stoppingToken).ConfigureAwait(false);
	}
}