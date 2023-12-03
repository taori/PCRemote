namespace Amusoft.PCR.AM.Service.Interfaces;


public interface IBackgroundService : IDisposable
{
	Task ExecuteAsync(CancellationToken stoppingToken);

	Task StartAsync(CancellationToken cancellationToken);

	Task StopAsync(CancellationToken cancellationToken);
}

