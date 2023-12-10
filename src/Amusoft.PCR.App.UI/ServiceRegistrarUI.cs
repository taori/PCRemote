#region

#region

using Amusoft.PCR.AM.UI.Extensions;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.App.UI.Extensions;
using Amusoft.PCR.App.UI.Implementations;
using Amusoft.PCR.Int.UI;

#endregion

#if ANDROID
using Amusoft.PCR.UI.App;
#endif

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
		services.AddSingleton<IAndroidResourceBridge, AndroidResourceBridge>();
#endif

		services.AddSingleton<ITypedNavigator, TypedNavigator>();

		services.AddScoped<ICredentialUserPrompt, HostCredentialUserPrompt>();
	}
}