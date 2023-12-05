using Android.App;

namespace Amusoft.PCR.Int.UI.Platforms.Android.Notifications;

public class NotificationChannelDeclaration
{
	public static readonly NotificationChannelDeclaration General =
		new(
			Microsoft.Maui.ApplicationModel.Platform.AppContext.GetString(Resource.String.notification_channel_general_name),
			Microsoft.Maui.ApplicationModel.Platform.AppContext.GetString(Resource.String.notification_channel_general_id),
			Microsoft.Maui.ApplicationModel.Platform.AppContext.GetString(Resource.String.notification_channel_general_description),
			NotificationImportance.Default);

	public static readonly NotificationChannelDeclaration Shutdown =
		new(
			Microsoft.Maui.ApplicationModel.Platform.AppContext.GetString(Resource.String.notification_channel_shutdown_id),
			Microsoft.Maui.ApplicationModel.Platform.AppContext.GetString(Resource.String.notification_channel_shutdown_name),
			Microsoft.Maui.ApplicationModel.Platform.AppContext.GetString(Resource.String.notification_channel_shutdown_description),
			NotificationImportance.High);

	public static readonly NotificationChannelDeclaration Restart =
		new(
			Microsoft.Maui.ApplicationModel.Platform.AppContext.GetString(Resource.String.notification_channel_restart_id),
			Microsoft.Maui.ApplicationModel.Platform.AppContext.GetString(Resource.String.notification_channel_restart_name),
			Microsoft.Maui.ApplicationModel.Platform.AppContext.GetString(Resource.String.notification_channel_restart_description),
			NotificationImportance.High);

	public static readonly NotificationChannelDeclaration Hibernate =
		new(
			Microsoft.Maui.ApplicationModel.Platform.AppContext.GetString(Resource.String.notification_channel_hibernate_id),
			Microsoft.Maui.ApplicationModel.Platform.AppContext.GetString(Resource.String.notification_channel_hibernate_name),
			Microsoft.Maui.ApplicationModel.Platform.AppContext.GetString(Resource.String.notification_channel_hibernate_description),
			NotificationImportance.Default);

	public static readonly Dictionary<NotificationChannelType, NotificationChannelDeclaration> All = new()
	{
		{NotificationChannelType.General, General},
		{NotificationChannelType.Shutdown, Shutdown},
		{NotificationChannelType.Restart, Restart},
		{NotificationChannelType.Hibernate, Hibernate},
	};

	public NotificationChannelDeclaration(string id, string name, string description, NotificationImportance importance)
	{
		Id = id;
		Name = name;
		Description = description;
		Importance = importance;
	}

	public string Id { get; }
	public string Name { get; }
	public string Description { get; }
	public NotificationImportance Importance { get; }
}