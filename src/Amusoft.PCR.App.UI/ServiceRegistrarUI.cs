using Amusoft.PCR.AM.UI;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.App.UI.Dependencies;
using Amusoft.PCR.App.UI.Extensions;
using Amusoft.PCR.Int.UI;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
		IntegrationDependencies.Dependencies.Add(svc => svc.TryAddSingleton<IAndroidResourceBridge, AndroidResourceBridge>());
		IntegrationDependencies.Dependencies.Add(svc => svc.AddHttpClient());
		IntegrationDependencies.Apply(services);
#endif

		services.TryAddTransient<ITypedNavigator, TypedNavigator>();
	}
}