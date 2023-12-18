using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Int.UI.Platforms.Android.Notifications;
using Amusoft.PCR.Int.UI.Platforms.Android.SystemState;
using AndroidX.Work;
using MauiPlatform = Microsoft.Maui.ApplicationModel.Platform;

namespace Amusoft.PCR.Int.UI.Platform.DelayedSystemState;

public class DelayedSystemStateWorker : IDelayedSystemStateWorker
{
	private readonly IHostCredentials _hostCredential;
	private readonly IAndroidResourceBridge _androidResourceBridge;

	public DelayedSystemStateWorker(IHostCredentials hostCredential, IAndroidResourceBridge androidResourceBridge)
	{
		_hostCredential = hostCredential;
		_androidResourceBridge = androidResourceBridge;
	}
	
	public async Task ShutdownAtAsync(DateTimeOffset scheduleAt, bool force)
	{
		if (await Permissions.CheckStatusAsync<NotificationPermission>() != PermissionStatus.Granted)
		{
			await Permissions.RequestAsync<NotificationPermission>();
		}

		StartDelayedWorker(scheduleAt, DelayedStateType.Shutdown, force);
	}

	public async Task RestartAtAsync(DateTimeOffset scheduleAt, bool force)
	{
		if (await Permissions.CheckStatusAsync<NotificationPermission>() != PermissionStatus.Granted)
		{
			await Permissions.RequestAsync<NotificationPermission>();
		}

		StartDelayedWorker(scheduleAt, DelayedStateType.Restart, force);
	}

	public async Task HibernateAtAsync(DateTimeOffset scheduleAt)
	{
		if (await Permissions.CheckStatusAsync<NotificationPermission>() != PermissionStatus.Granted)
		{
			await Permissions.RequestAsync<NotificationPermission>();
		}

		StartDelayedWorker(scheduleAt, DelayedStateType.Hibernate, false);
	}

	public void Clear()
	{
		WorkManager.GetInstance(MauiPlatform.AppContext).CancelAllWorkByTag(_hostCredential.Address.ToString());
	}

	private void StartDelayedWorker(DateTimeOffset scheduleAt, DelayedStateType stateType, bool force)
	{
		var workerId = $"{_hostCredential.Address}.{stateType}";
		var dataBuilder = new Data.Builder()
			.PutInt(DelayedWorker.InputParameters.DelayedActionType, (int)stateType)
			.PutString(DelayedWorker.InputParameters.FinalizeActionAt, scheduleAt.ToString())
			.PutString(DelayedWorker.InputParameters.HostName, _hostCredential.Title)
			.PutString(DelayedWorker.InputParameters.Address, _hostCredential.Address.ToString())
			.PutString(DelayedWorker.InputParameters.Protocol, _hostCredential.Protocol)
			.PutString(DelayedWorker.InputParameters.LocalizationAbort, _androidResourceBridge.MessageAbort)
			.PutString(DelayedWorker.InputParameters.LocalizationRestart, _androidResourceBridge.MessageRestart_0)
			.PutString(DelayedWorker.InputParameters.LocalizationShutdown, _androidResourceBridge.MessageShutdown_0)
			.PutString(DelayedWorker.InputParameters.LocalizationHibernate, _androidResourceBridge.MessageHibernate_0)
			.PutBoolean(DelayedWorker.InputParameters.ExecuteWithForce, force);

		var constraints = new Constraints.Builder()
			.SetRequiredNetworkType(NetworkType.Connected!);
		var request = OneTimeWorkRequest.Builder.From<DelayedWorker>()
			.SetInputData(dataBuilder.Build())
			.SetConstraints(constraints.Build())
			.AddTag(workerId)
			.AddTag(_hostCredential.Address.ToString())
			.Build();
		var workManager = WorkManager.GetInstance(MauiPlatform.AppContext);
		workManager.EnqueueUniqueWork(workerId, ExistingWorkPolicy.Keep!, request);
	}
}