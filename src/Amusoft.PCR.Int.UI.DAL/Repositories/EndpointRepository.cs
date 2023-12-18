using System.Net;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Domain.UI.Entities;
using Amusoft.PCR.Int.UI.DAL.Database;
using Microsoft.EntityFrameworkCore;

namespace Amusoft.PCR.Int.UI.DAL.Repositories;

internal class EndpointRepository : IEndpointRepository
{
	private readonly UiDbContext _dbContext;

	public EndpointRepository(UiDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<Endpoint?> TryGetEndpointAsync(IPEndPoint endPoint, CancellationToken cancellationToken)
	{
		var match = await _dbContext.Endpoints
			.FirstOrDefaultAsync(d => d.Address == endPoint.ToString(), cancellationToken)
			.ConfigureAwait(false);

		return match;
	}

	public async Task<Endpoint> CreateEndpointAsync(IPEndPoint endPoint, CancellationToken cancellationToken)
	{
		var item = new Endpoint() { Address = endPoint.ToString() };
		_dbContext.Endpoints.Add(item);
		var changes = await _dbContext.SaveChangesAsync(cancellationToken);
		if (changes <= 0 || item.Id.Equals(Guid.Empty))
			throw new DatabaseException("No insert happened");

		return item;
	}

	public async Task<EndpointAccount?> TryGetEndpointAccountAsync(Guid endPointId, string email, CancellationToken cancellationToken)
	{
		var match = await _dbContext.EndpointAccounts
			.Include(d => d.BearerTokens)
			.FirstOrDefaultAsync(d => d.Email == email && d.EndpointId == endPointId, cancellationToken)
			.ConfigureAwait(false);

		return match;
	}

	public async Task<EndpointAccount> CreateEndpointAccountAsync(Guid endPointId, string email, CancellationToken cancellationToken)
	{
		var item = new EndpointAccount() { Email = email, EndpointId = endPointId };
		_dbContext.EndpointAccounts.Add(item);
		var changes = await _dbContext.SaveChangesAsync(cancellationToken);
		if (changes <= 0 || item.Id.Equals(Guid.Empty))
			throw new DatabaseException("No insert happened");

		return item;
	}

	public Task<EndpointAccount[]> GetEndpointAccountsAsync(IPEndPoint endPoint, CancellationToken cancellationToken)
	{
		return _dbContext.EndpointAccounts
			.Where(d => d.Endpoint.Address == endPoint.ToString())
			.Include(d => d.BearerTokens)
			.ToArrayAsync(cancellationToken);
	}

	public Task<EndpointAccount> GetEndpointAccountAsync(Guid endpointAccountId, CancellationToken cancellationToken)
	{
		return _dbContext.EndpointAccounts
			.Include(d => d.BearerTokens)
			.FirstAsync(d => d.Id == endpointAccountId, cancellationToken);
	}

	public Task<int> RemoveEndpointAccountAsync(Guid endpointAccountId)
	{
		return _dbContext.EndpointAccounts
			.Where(d => d.Id == endpointAccountId)
			.ExecuteDeleteAsync();
	}

	public Task<int> RemoveEndpointAsync(Guid endpointId)
	{
		return _dbContext.Endpoints
			.Where(d => d.Id == endpointId)
			.ExecuteDeleteAsync();
	}
}