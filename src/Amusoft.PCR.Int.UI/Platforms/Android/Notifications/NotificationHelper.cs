using Android.App;
using Android.Content;

namespace Amusoft.PCR.Int.UI.Platforms.Android.Notifications;

public class NotificationHelper
{
	public static void UpdateNotification(int notificationId, Notification.Builder builder)
	{
		var notificationManager = Microsoft.Maui.ApplicationModel.Platform.AppContext.GetSystemService(Context.NotificationService) as NotificationManager;
		notificationManager?.Notify(notificationId, builder.Build());
	}

	public static Notification.Builder? DisplayNotification(int notificationId, NotificationChannelType channel, Action<Notification.Builder> builder)
	{
		if (!OperatingSystem.IsAndroidVersionAtLeast(26))
			return null;
		var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
		if (activity is null)
			return null;
		var notificationManager = activity.GetSystemService(Context.NotificationService) as NotificationManager;
		if (notificationManager is null)
			return null;

		var builderInstance = new Notification.Builder(activity, NotificationChannelDeclaration.All[channel].Id);
		builderInstance.SetSmallIcon(Resource.Drawable.outline_power_settings_new_24);
		
		builder(builderInstance);

		var notification = builderInstance.Build();
		notificationManager.Notify(notificationId, notification);

		return builderInstance;
	}

	public static void SetupNotificationChannels()
	{
		// Create the NotificationChannel, but only on API 26+ because
		// the NotificationChannel class is new and not in the support library
		if (Microsoft.Maui.ApplicationModel.Platform.AppContext.GetSystemService(Context.NotificationService) is NotificationManager notificationManager && OperatingSystem.IsAndroidVersionAtLeast(26))
		{
			foreach (var (_, channelDeclaration) in NotificationChannelDeclaration.All)
			{
				var channel = new NotificationChannel(channelDeclaration.Id, channelDeclaration.Name, channelDeclaration.Importance)
				{
					Description = channelDeclaration.Description
				};
				notificationManager.CreateNotificationChannel(channel);
			}
		}
	}

	public static void DestroyNotification(int id)
	{
		var currentActivity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
		var notificationManager = currentActivity?.GetSystemService(Context.NotificationService) as NotificationManager;
		notificationManager?.Cancel(id);
	}

	public static void ClearNotifications()
	{
		var currentActivity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
		var notificationManager = currentActivity?.GetSystemService(Context.NotificationService) as NotificationManager;
		notificationManager?.CancelAll();
	}
}