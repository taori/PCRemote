using Android.App;
using Android.Content;
using AndroidX.Work;
using NLog;
using Logger = NLog.Logger;

namespace Amusoft.PCR.Int.UI.Platforms.Android.SystemState;

[BroadcastReceiver(Exported = false)]
[IntentFilter(new[] { ActionKindHibernate, ActionKindRestart, ActionKindShutdown })]

public class DelayedSystemStateBroadcastReceiver : BroadcastReceiver
{
	public static readonly DelayedSystemStateBroadcastReceiver Instance = new();

	private static readonly Logger Log = LogManager.GetCurrentClassLogger();

	public const string ActionKindHibernate = nameof(ActionKindHibernate);
	public const string ActionKindRestart = nameof(ActionKindRestart);
	public const string ActionKindShutdown = nameof(ActionKindShutdown);
	public const string InputWorkerTag = nameof(InputWorkerTag);
	public const string InputNotificationId = nameof(InputNotificationId);

	public override void OnReceive(Context? context, Intent? intent)
	{
		try
		{
			var bundle = intent?.Extras;
			var notificationId = bundle?.GetInt(InputNotificationId, -1) ?? throw new Exception($"Data missing from intent for {nameof(InputNotificationId)}");
			var workerTag = bundle.GetString(InputWorkerTag) ?? throw new Exception($"Data missing from intent for {nameof(InputWorkerTag)}");

			Log.Debug("Received intent {Action} {NotificationId} {WorkerTag}", intent?.Action ?? "Unknown action", notificationId, workerTag);
			
			if (WorkManager.GetInstance(Microsoft.Maui.ApplicationModel.Platform.AppContext) is {} workManager)
			{
				Log.Debug("Aborting with Tag {Tag}", workerTag);
				workManager.CancelAllWorkByTag(workerTag);
			}
		}
		catch (Exception e)
		{
			Log.Error(e, "Failed to process broadcast");
		}
	}
}