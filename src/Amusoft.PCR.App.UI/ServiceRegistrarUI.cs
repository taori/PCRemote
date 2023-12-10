#region

#region

#region

using Amusoft.PCR.AM.UI.Extensions;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.App.UI.Extensions;
using Amusoft.PCR.App.UI.Implementations;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Amusoft.PCR.Int.UI;

#endregion

#if ANDROID
using Amusoft.PCR.UI.App;
#endif

#endregion

#endregion

namespace Amusoft.PCR.App.UI;

internal static class ServiceRegistrarUI
{
	public static void Register(IServiceCollection services)
	{
		// This switch must be set before creating the GrpcChannel/HttpClient.
		// AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

		services.AddHttpClient();

		services.AddUIApplicationModel();
		services.AddUIIntegration();
		services.AddUIViews();

#if ANDROID
		services.TryAddSingleton<IAndroidResourceBridge, AndroidResourceBridge>();
#endif

		services.TryAddTransient<ITypedNavigator, TypedNavigator>();
		services.TryAddSingleton<ICredentialUserPrompt, HostCredentialUserPrompt>();
	}
}