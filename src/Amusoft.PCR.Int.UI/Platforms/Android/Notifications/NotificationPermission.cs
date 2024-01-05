using Android;

namespace Amusoft.PCR.Int.UI.Platforms.Android.Notifications;

internal class NotificationPermission : Permissions.BasePlatformPermission
{
	public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
		new List<(string androidPermission, bool isRuntime)>
		{
#if ANDROID33_0_OR_GREATER
			(Manifest.Permission.PostNotifications, true)
#endif
		}.ToArray();
}