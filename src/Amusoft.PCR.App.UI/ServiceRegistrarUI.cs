#region

using Amusoft.PCR.AM.UI.Extensions;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.App.UI.Extensions;
using Amusoft.PCR.App.UI.Implementations;
using Amusoft.PCR.Int.UI;
using Microsoft.EntityFrameworkCore;

#endregion

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

		services.AddSingleton<DbContextOptions>(sp => new DbContextOptionsBuilder()
			.UseSqlite($"Data Source={Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}pcr3.db")
			.Options);

#if ANDROID
		services.AddSingleton<IAndroidResourceBridge, AndroidResourceBridge>();
#endif

		services.AddSingleton<ITypedNavigator, TypedNavigator>();
	}
}