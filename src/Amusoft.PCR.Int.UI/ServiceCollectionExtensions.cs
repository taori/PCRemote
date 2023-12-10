#region

#region

#region

using Amusoft.PCR.AM.Shared.Interfaces;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Int.IPC;
using Amusoft.PCR.Int.UI.DAL;
using Amusoft.PCR.Int.UI.Platform.DelayedSystemState;
using Amusoft.PCR.Int.UI.ProjectDepencies;
using Amusoft.PCR.Int.UI.Shared;

#endregion

#if ANDROID
using Android.Content;
using Amusoft.PCR.Int.UI.Platforms.Android.Notifications;
using Amusoft.PCR.Int.UI.Platforms.Android.SystemState;
#endif

#endregion

#endregion

namespace Amusoft.PCR.Int.UI;

public static class ServiceCollectionExtensions
{
	public static void AddUIIntegration(this IServiceCollection services)
	{
		services.AddInterprocessCommunication();
		services.AddUIDataLayer();
		
		services.AddSingleton<IToast, Toast>();
		services.AddSingleton<IAgentEnvironment, AgentEnvironment>();
		services.AddSingleton<IUserInterfaceService, UserInterfaceService>();
		services.AddSingleton<IFileStorage, FileStorage>();
		services.AddSingleton<IUserAccountManagerFactory, UserAccountManagerFactory>();

		services.AddTransient<IGrpcChannelFactory, GrpcChannelFactory>();
		services.AddTransient<IDelayedSystemStateWorker, DelayedSystemStateWorker>();
		services.AddTransient<IBearerTokenProvider, BearerTokenProvider>();
		services.AddTransient<IDesktopIntegrationServiceFactory, DesktopIntegrationServiceFactory>();


#if ANDROID
		NotificationHelper.SetupNotificationChannels();

		Microsoft.Maui.ApplicationModel.Platform.AppContext.RegisterReceiver(DelayedSystemStateBroadcastReceiver.Instance, new IntentFilter(DelayedSystemStateBroadcastReceiver.ActionKindHibernate));
		Microsoft.Maui.ApplicationModel.Platform.AppContext.RegisterReceiver(DelayedSystemStateBroadcastReceiver.Instance, new IntentFilter(DelayedSystemStateBroadcastReceiver.ActionKindRestart));
		Microsoft.Maui.ApplicationModel.Platform.AppContext.RegisterReceiver(DelayedSystemStateBroadcastReceiver.Instance, new IntentFilter(DelayedSystemStateBroadcastReceiver.ActionKindShutdown));
#endif
	}
}