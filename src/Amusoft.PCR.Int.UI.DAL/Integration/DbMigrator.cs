#region

using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Int.UI.DAL.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

#endregion

namespace Amusoft.PCR.Int.UI.DAL.Integration;

internal class DbMigrator(ILogger<DbMigrator> Log, UiDbContext DbContext) : IMainInitializer
{
	public Task ApplyAsync()
	{
		Log.LogDebug("Migrating database");
		return DbContext.Database.MigrateAsync();
	}
}