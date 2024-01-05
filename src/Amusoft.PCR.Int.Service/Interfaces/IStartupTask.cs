namespace Amusoft.PCR.Int.Service.Interfaces;

public interface IStartupTask
{
	public int Priority { get; }
	Task ExecuteAsync(CancellationToken stoppingToken);
}