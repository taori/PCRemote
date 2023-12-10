using System.Net;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Domain.UI.Entities;
using Amusoft.PCR.Int.UI.DAL.Database;
using Microsoft.EntityFrameworkCore;

namespace Amusoft.PCR.Int.UI.DAL.Repositories;

internal class BearerTokenRepository : IBearerTokenStorage
{
	private readonly UiDbContext _dbContext;

	public BearerTokenRepository(UiDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<BearerToken?> GetLatestTokenAsync(IPEndPoint endPoint, CancellationToken cancellationToken)
	{
		var match = await _dbContext.BearerTokens
			.AsNoTracking()
			.OrderByDescending(d => d.Expires)
			.FirstOrDefaultAsync(d => d.Address == endPoint.ToString(), cancellationToken)
			.ConfigureAwait(false);

		return match;
	}

	public async Task<bool> AddTokenAsync(IPEndPoint endPoint, BearerToken token, CancellationToken cancellationToken)
	{
		_dbContext.BearerTokens.Add(token);

		return await _dbContext
			.SaveChangesAsync(cancellationToken)
			.ConfigureAwait(false) > 0;
	}

	public async Task<bool> PruneAsync(IPEndPoint ipEndPoint, CancellationToken cancellationToken)
	{
		var matches = await _dbContext.BearerTokens
			.Where(d => d.Address == ipEndPoint.ToString())
			.ToListAsync(cancellationToken)
			.ConfigureAwait(false);

		_dbContext.BearerTokens.RemoveRange(matches);
		return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
	}
}