#region

using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Int.UI.DAL.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

#endregion

namespace Amusoft.PCR.Int.UI.DAL.Integration;

internal class DbMigrator(ILogger<DbMigrator> Log, UiDbContext DbContext) : IMainInitializer
{
	public async Task ApplyAsync()
	{
		Log.LogDebug("Migrating database");
		await DbContext.Database.MigrateAsync();
		var canConnect = await DbContext.Database.CanConnectAsync();
		if (!canConnect)
			Log.LogError("Database unable to connect");
	}
}