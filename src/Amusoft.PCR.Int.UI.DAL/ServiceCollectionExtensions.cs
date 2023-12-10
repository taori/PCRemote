#region

using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Int.UI.DAL.Database;
using Amusoft.PCR.Int.UI.DAL.Integration;
using Amusoft.PCR.Int.UI.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

#endregion

namespace Amusoft.PCR.Int.UI.DAL;

public static class ServiceCollectionExtensions
{
	public static void AddUIDataLayer(this IServiceCollection services)
	{
		var ds = Path.DirectorySeparatorChar;
		services.AddDbContext<UiDbContext>((provider, builder) => builder
				.UseSqlite($"Data Source={Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}{ds}pcr3.db")
				.UseLoggerFactory(provider.GetRequiredService<ILoggerFactory>())
				.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
#if DEBUG
				.EnableDetailedErrors()
				.EnableSensitiveDataLogging()
#endif
			, contextLifetime: ServiceLifetime.Transient
			, optionsLifetime: ServiceLifetime.Singleton
		);

		services.AddScoped<IHostRepository, HostRepository>();
		services.AddScoped<IClientSettingsRepository, ClientSettingsRepository>();

		services.TryAddTransient<IBearerTokenStorage, BearerTokenRepository>();
		services.TryAddTransient<IMainInitializer, DbMigrator>();
	}
}