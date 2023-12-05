using Amusoft.PCR.AM.Shared.Interfaces;
using Amusoft.PCR.AM.UI.Extensions;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.Repositories;
using Amusoft.PCR.AM.UI.ViewModels;
using Amusoft.PCR.App.UI.Extensions;
using Amusoft.PCR.App.UI.Implementations;
using Amusoft.PCR.App.UI.Pages;
using Amusoft.PCR.Int.UI;
using INavigation = Amusoft.PCR.AM.UI.Interfaces.INavigation;

#if ANDROID
using Amusoft.PCR.UI.App;
#endif

namespace Amusoft.PCR.App.UI;

internal static class ServiceRegistrarUI
{
	public static void Register(IServiceCollection services)
	{
		// This switch must be set before creating the GrpcChannel/HttpClient.
		// AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

		services.AddUIApplicationModel();
		services.AddUIIntegration();
		services.AddUIViews();

#if ANDROID
		services.AddSingleton<IAndroidResourceBridge, AndroidResourceBridge>();
#endif

		services.AddSingleton<ITypedNavigator, TypedNavigator>();
	}
}