using System.Net;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Domain.UI.Entities;
using Amusoft.PCR.Int.UI.DAL.Database;
using Microsoft.EntityFrameworkCore;

namespace Amusoft.PCR.Int.UI.DAL.Repositories;

internal class BearerTokenRepository : IBearerTokenRepository
{
	private readonly UiDbContext _dbContext;

	public BearerTokenRepository(UiDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<BearerToken?> GetLatestTokenAsync(Guid endpointAccountId, CancellationToken cancellationToken)
	{
		var match = await _dbContext.BearerTokens
			.AsNoTracking()
			.OrderByDescending(d => d.IssuedAt)
			.FirstOrDefaultAsync(d => d.EndpointAccountId == endpointAccountId, cancellationToken)
			.ConfigureAwait(false);

		return match;
	}

	public async Task<bool> AddTokenAsync(BearerToken token, CancellationToken cancellationToken)
	{
		_dbContext.BearerTokens.Add(token);

		return await _dbContext
			.SaveChangesAsync(cancellationToken)
			.ConfigureAwait(false) > 0;
	}

	public async Task<bool> DeleteAsync(IPEndPoint endPoint, CancellationToken cancellationToken)
	{
		return await _dbContext.BearerTokens
			.Where(d => d.EndpointAccount.Endpoint.Address.Equals(endPoint.ToString()))
			.ExecuteDeleteAsync(cancellationToken)
			.ConfigureAwait(false) > 0;
	}
}