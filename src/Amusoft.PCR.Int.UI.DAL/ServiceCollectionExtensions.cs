#region

using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Int.UI.DAL.Database;
using Amusoft.PCR.Int.UI.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace Amusoft.PCR.Int.UI.DAL;

public static class ServiceCollectionExtensions
{
	public static void AddUIDataLayer(this IServiceCollection services)
	{
		services.AddScoped<UiDbContext>();

		services.AddScoped<IBearerTokenStorage, BearerTokenRepository>();
	}
}