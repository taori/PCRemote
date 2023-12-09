#region

using System.Net;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Domain.UI.Entities;
using Amusoft.PCR.Int.UI.DAL.Database;
using Microsoft.EntityFrameworkCore;

#endregion

namespace Amusoft.PCR.Int.UI.DAL.Repositories;

internal class BearerTokenRepository : IBearerTokenStorage
{
	private readonly UiDbContext _dbContext;

	public BearerTokenRepository(UiDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public BearerToken? GetTokenAsync(IPEndPoint endPoint, CancellationToken cancellationToken)
	{
		var match = _dbContext.BearerTokens
			.AsNoTracking()
			.OrderByDescending(d => d.Expires)
			.FirstOrDefault(d => d.Address == endPoint.ToString());

		return match;
	}

	public async Task<bool> AddTokenAsync(IPEndPoint endPoint, BearerToken token, CancellationToken cancellationToken)
	{
		_dbContext.BearerTokens.Add(token);
		return await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false) > 0;
	}
}