using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Int.UI.DAL.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.UI.DAL.Integration;

internal class DbMigrator : IMainInitializer
{
	private readonly ILogger<DbMigrator> _log;
	private readonly UiDbContext _dbContext;

	public DbMigrator(ILogger<DbMigrator> log, UiDbContext dbContext)
	{
		_log = log;
		_dbContext = dbContext;
	}
	
	public async Task ApplyAsync()
	{
		_log.LogDebug("Migrating database");
		await _dbContext.Database.MigrateAsync();
		var canConnect = await _dbContext.Database.CanConnectAsync();
		if (!canConnect)
			_log.LogError("Database unable to connect");
	}
}