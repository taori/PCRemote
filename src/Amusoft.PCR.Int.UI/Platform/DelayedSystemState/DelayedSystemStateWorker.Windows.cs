using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Domain.Shared.Interfaces;

namespace Amusoft.PCR.Int.UI.Platform.DelayedSystemState;

public class DelayedSystemStateWorker : IDelayedSystemStateWorker
{
	private readonly IDesktopClientMethods _desktopClientMethods;

	public DelayedSystemStateWorker(IDesktopClientMethods desktopClientMethods)
	{
		_desktopClientMethods = desktopClientMethods;
	}
	
	public Task ShutdownAtAsync(DateTimeOffset scheduleAt, bool force)
	{
		var diff = scheduleAt - DateTimeOffset.Now;
		if (diff.Ticks > 0)
		{
			return _desktopClientMethods.Shutdown(diff, force);
		}

		return Task.CompletedTask;
	}

	public Task RestartAtAsync(DateTimeOffset scheduleAt, bool force)
	{
		var diff = scheduleAt - DateTimeOffset.Now;
		if (diff.Ticks > 0)
		{
			return _desktopClientMethods.Restart(diff, force);
		}

		return Task.CompletedTask;
	}

	public Task HibernateAtAsync(DateTimeOffset scheduleAt)
	{
		var diff = scheduleAt - DateTimeOffset.Now;
		if (diff.Ticks > 0)
		{
			return _desktopClientMethods.Hibernate();
		}

		return Task.CompletedTask;
	}
}