namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IDelayedSystemStateWorker
{
	Task ShutdownAtAsync(DateTimeOffset scheduleAt, bool force);
	
	Task RestartAtAsync(DateTimeOffset scheduleAt, bool force);
	
	Task HibernateAtAsync(DateTimeOffset scheduleAt);
}