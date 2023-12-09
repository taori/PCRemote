#region

using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Int.UI.DAL.Database;

#endregion

namespace Amusoft.PCR.Int.UI.DAL.Repositories;

internal class BearerTokenRepository : IBearerTokenStorage
{
	private readonly UiDbContext _dbContext;

	public BearerTokenRepository(UiDbContext dbContext)
	{
		_dbContext = dbContext;
	}
}