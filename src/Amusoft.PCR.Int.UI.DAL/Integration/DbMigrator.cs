using System.Diagnostics;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Int.UI.DAL.Database;
using Microsoft.EntityFrameworkCore;

namespace Amusoft.PCR.Int.UI.DAL.Integration;

internal class DbMigrator : IApplicationStartup
{
	private readonly UiDbContext _dbContext;

	public DbMigrator(UiDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public void Apply()
	{
		Debug.WriteLine("Applying migrations");
		_dbContext.Database.Migrate();
		var canConnect = _dbContext.Database.CanConnect();
		if (!canConnect)
			Debug.WriteLine("Database unable to connect");
	}
}