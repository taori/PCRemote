namespace Amusoft.PCR.Int.UI.Platforms.Android.Notifications;

internal class NotificationPermission : Permissions.BasePlatformPermission
{
	public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
		new List<(string androidPermission, bool isRuntime)>
		{
			(global::Android.Manifest.Permission.PostNotifications, true)
		}.ToArray();
}