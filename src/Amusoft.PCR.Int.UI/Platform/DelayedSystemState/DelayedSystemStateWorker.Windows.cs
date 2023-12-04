using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Domain.Shared.Interfaces;

namespace Amusoft.PCR.Int.UI.Platform.DelayedSystemState;

public class DelayedSystemStateWorker : IDelayedSystemStateWorker
{
	private readonly IIpcIntegrationService _ipcService;

	public DelayedSystemStateWorker(IIpcIntegrationService ipcService)
	{
		_ipcService = ipcService;
	}
	
	public Task ShutdownAtAsync(DateTimeOffset scheduleAt, bool force)
	{
		var diff = scheduleAt - DateTimeOffset.Now;
		if (diff.Ticks > 0)
		{
			return _ipcService.DesktopClient.Shutdown(diff, force);
		}

		return Task.CompletedTask;
	}

	public Task RestartAtAsync(DateTimeOffset scheduleAt, bool force)
	{
		var diff = scheduleAt - DateTimeOffset.Now;
		if (diff.Ticks > 0)
		{
			return _ipcService.DesktopClient.Restart(diff, force);
		}

		return Task.CompletedTask;
	}

	public Task HibernateAtAsync(DateTimeOffset scheduleAt)
	{
		var diff = scheduleAt - DateTimeOffset.Now;
		if (diff.Ticks > 0)
		{
			return _ipcService.DesktopClient.Hibernate();
		}

		return Task.CompletedTask;
	}

	public void Clear()
	{
	}
}