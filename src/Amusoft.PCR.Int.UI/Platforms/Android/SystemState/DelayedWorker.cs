using System.Net;
using Amusoft.PCR.Int.UI.Platforms.Android.Notifications;
using Amusoft.PCR.Int.UI.ProjectDepencies;
using Android.App;
using Android.Content;
using AndroidX.Work;
using NLog;
using Exception = System.Exception;
using Logger = NLog.Logger;

namespace Amusoft.PCR.Int.UI.Platforms.Android.SystemState;

internal class DelayedWorker : Worker
{
	private static readonly Logger Log = LogManager.GetCurrentClassLogger();
	private readonly string _locAbort;
	private readonly string _locRestart;
	private readonly string _locShutdown;
	private readonly string _locHibernate;

	public static class InputParameters
	{
		public const string Address = "Adress";
		public const string Protocol = "Protocol";
		public const string DelayedActionType = "DelayedActionType";
		public const string FinalizeActionAt = "FinalizeActionAt";
		public const string HostName = "HostName";
		public const string ExecuteWithForce = "ExecuteWithForce";
		public const string LocalizationAbort = "LocalizationAbort";
		public const string LocalizationRestart = "LocalizationRestart";
		public const string LocalizationHibernate = "LocalizationHibernate";
		public const string LocalizationShutdown = "LocalizationShutdown";
	}

	public DelayedWorker(Context context, WorkerParameters workerParams) : base(context, workerParams)
	{
		_locAbort = InputData.GetString(InputParameters.LocalizationAbort) ?? throw new Exception(nameof(InputParameters.LocalizationAbort));
		_locRestart = InputData.GetString(InputParameters.LocalizationRestart) ?? throw new Exception(nameof(InputParameters.LocalizationRestart));
		_locShutdown = InputData.GetString(InputParameters.LocalizationShutdown) ?? throw new Exception(nameof(InputParameters.LocalizationShutdown));
		_locHibernate= InputData.GetString(InputParameters.LocalizationHibernate) ?? throw new Exception(nameof(InputParameters.LocalizationHibernate));
	}

	public override Result DoWork()
	{
		return DoWorkAsync().GetAwaiter().GetResult();
	}

	private async Task<Result> DoWorkAsync()
	{
		try
		{
			var actionType = (DelayedStateType)InputData.GetInt(InputParameters.DelayedActionType, -1);
			var protocol = InputData.GetString(InputParameters.Protocol) ?? throw new Exception("Protocol is null");
			var address = InputData.GetString(InputParameters.Address)?? throw new Exception("address is null");
			var finalizeActionAt = InputData.GetString(InputParameters.FinalizeActionAt) ?? throw new Exception("finalizeActionAt is null");
			var force = InputData.GetBoolean(InputParameters.ExecuteWithForce, false);
			if (string.IsNullOrEmpty(address) || !IPEndPoint.TryParse(address, out var addressParsed))
			{
				Log.Error("Failed to parse address");
				return Result.InvokeFailure();
			}
			if (string.IsNullOrEmpty(finalizeActionAt) || !DateTimeOffset.TryParse(finalizeActionAt, out var finalizeActionAtParsed))
			{
				Log.Error("Failed to parse finalizeActionAt");
				return Result.InvokeFailure();
			}
			if (Tags.Count != 3)
			{
				Log.Error("Tag count mismatch");
				return Result.InvokeFailure();
			}
		
			var notificationId = $"{actionType}+{addressParsed}".GetHashCode();
			var intentAction = GetIntentAction(actionType);
			var intent = new Intent(intentAction);
			var tagValue = Tags.Skip(1).First();

			intent.PutExtra(DelayedSystemStateBroadcastReceiver.InputWorkerTag, tagValue);
			intent.PutExtra(DelayedSystemStateBroadcastReceiver.InputNotificationId, notificationId);

			var start = DateTimeOffset.Now;
			var progressMax = (int)(finalizeActionAtParsed - start).TotalSeconds;

			if (progressMax < 0)
			{
				Log.Debug("Shutdown is in the past - cancelling");
				return Result.InvokeSuccess();
			}

			var notificationChannel = GetNotificationChannel(actionType);

			var notificationBuilder = NotificationHelper.DisplayNotification(notificationId, notificationChannel, builder =>
			{
				var abortIntent = PendingIntent.GetBroadcast(Microsoft.Maui.ApplicationModel.Platform.AppContext, 1, intent, PendingIntentFlags.Immutable);

				builder
					.SetProgress(progressMax, progressMax, false)
					.SetSmallIcon(GetNotificationIcon(actionType))
					.SetOnlyAlertOnce(true)
					.SetFlag((int) NotificationFlags.AutoCancel, false)
					.SetContentTitle(GetNotificationTitle(actionType))
					.SetContentText(DurationToTime(TimeSpan.FromSeconds(progressMax)))
					.SetActions(
						new Notification.Action(GetNotificationIcon(actionType), _locAbort, abortIntent)
					);
			});

			if (notificationBuilder is null)
				return Result.InvokeFailure();

			double diff;
			do
			{
				if (IsStopped)
				{
					Log.Debug("Worker was stopped");
					NotificationHelper.DestroyNotification(notificationId);
					return Result.InvokeSuccess();
				}

				diff = (finalizeActionAtParsed - DateTimeOffset.Now).TotalSeconds;
				notificationBuilder
					.SetProgress(progressMax, (int)diff, false)
					.SetContentText(DurationToTime(TimeSpan.FromSeconds(diff)));
				NotificationHelper.UpdateNotification(notificationId, notificationBuilder);

				if (diff > 0)
					await Task.Delay(1000).ConfigureAwait(false);

			} while (diff > 0);

			NotificationHelper.DestroyNotification(notificationId);

			var stateClient = new DelayedSystemStateClient(protocol, addressParsed);
			await ProcessFinalize(actionType, stateClient, force);

			return Result.InvokeSuccess();
		}
		catch (Exception e)
		{
			Log.Error(e, "An error occured while trying to execute the worker");
			return Result.InvokeFailure();
		}
	}

	private int GetNotificationIcon(DelayedStateType actionType)
	{
		return actionType switch
		{
			DelayedStateType.Shutdown => Resource.Drawable.outline_power_settings_new_24,
			DelayedStateType.Restart => Resource.Drawable.restart_alt_24px,
			DelayedStateType.Hibernate => Resource.Drawable.energy_savings_leaf_24px,
			_ => throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null)
		};
	}

	private NotificationChannelType GetNotificationChannel(DelayedStateType actionType)
	{
		return actionType switch
		{
			DelayedStateType.Shutdown => NotificationChannelType.Shutdown,
			DelayedStateType.Restart => NotificationChannelType.Restart,
			DelayedStateType.Hibernate => NotificationChannelType.Hibernate,
			_ => throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null)
		};
	}

	private static string DurationToTime(TimeSpan duration)
	{
		return duration.ToString("hh\\:mm\\:ss");
	}

	private static async Task ProcessFinalize(DelayedStateType actionType, DelayedSystemStateClient stateClient, bool force)
	{
		switch (actionType)
		{
			case DelayedStateType.Shutdown:
				await stateClient.ShutdownAsync(TimeSpan.FromMinutes(1), force);
				break;
			case DelayedStateType.Restart:
				await stateClient.RestartAsync(TimeSpan.FromMinutes(1), force);
				break;
			case DelayedStateType.Hibernate:
				await stateClient.HibernateAsync(TimeSpan.FromMinutes(1));
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private string GetNotificationTitle(DelayedStateType delayedStateType)
	{
		return delayedStateType switch
		{
			DelayedStateType.Shutdown => string.Format(_locShutdown, InputData.GetString(InputParameters.HostName)),
			DelayedStateType.Restart => string.Format(_locRestart, InputData.GetString(InputParameters.HostName)),
			DelayedStateType.Hibernate => string.Format(_locHibernate, InputData.GetString(InputParameters.HostName)),
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	private static string GetIntentAction(DelayedStateType actionType)
	{
		return actionType switch
		{
			DelayedStateType.Shutdown => DelayedSystemStateBroadcastReceiver.ActionKindShutdown,
			DelayedStateType.Restart => DelayedSystemStateBroadcastReceiver.ActionKindRestart,
			DelayedStateType.Hibernate => DelayedSystemStateBroadcastReceiver.ActionKindHibernate,
			_ => throw new ArgumentOutOfRangeException()
		};
	}
}