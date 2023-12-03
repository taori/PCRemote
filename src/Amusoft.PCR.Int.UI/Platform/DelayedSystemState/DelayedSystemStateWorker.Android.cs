using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Domain.Shared.Interfaces;
using Android.Content;
using Android.Runtime;
using AndroidX.Work;

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
		var workManager = WorkManager.GetInstance(Microsoft.Maui.ApplicationModel.Platform.AppContext);
		// OneTimeWorkRequest.Builder.From<DelayedWorker>().
		// workManager.EnqueueUniqueWork()
		throw new NotImplementedException();
	}

	public Task RestartAtAsync(DateTimeOffset scheduleAt, bool force)
	{
		throw new NotImplementedException();
	}

	public Task HibernateAtAsync(DateTimeOffset scheduleAt)
	{
		throw new NotImplementedException();
	}
}

internal class DelayedWorker : Worker
{
	public DelayedWorker(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
	{
	}

	public DelayedWorker(Context context, WorkerParameters workerParams) : base(context, workerParams)
	{
	}

	public override Result DoWork()
	{
		throw new NotImplementedException();
	}
}

internal enum DelayedStateType
{
	Shutdown,
	Restart,
	Hibernate
}