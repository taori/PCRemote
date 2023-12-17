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

	public async Task<Guid?> GetEndpointIdAsync(IPEndPoint endPoint)
	{
		var match = await _dbContext.Endpoints
			.FirstOrDefaultAsync(d => d.Address == endPoint.ToString())
			.ConfigureAwait(false);

		return match?.Id;
	}

	public async Task<Guid> CreateEndpointAsync(IPEndPoint endPoint)
	{
		var item = new Endpoint() { Address = endPoint.ToString() };
		_dbContext.Endpoints.Add(item);
		var changes = await _dbContext.SaveChangesAsync();
		if (changes <= 0 || item.Id.Equals(Guid.Empty))
			throw new DatabaseException("No insert happened");

		return item.Id;
	}

	public async Task<Guid?> GetEndpointAccountIdAsync(Guid endPointId, string email)
	{
		var match = await _dbContext.EndpointAccounts
			.FirstOrDefaultAsync(d => d.Email == email && d.EndpointId == endPointId)
			.ConfigureAwait(false);

		return match?.Id;
	}

	public async Task<Guid> CreateEndpointAccountAsync(Guid endPointId, string email)
	{
		var item = new EndpointAccount() { Email = email, EndpointId = endPointId };
		_dbContext.EndpointAccounts.Add(item);
		var changes = await _dbContext.SaveChangesAsync();
		if (changes <= 0 || item.Id.Equals(Guid.Empty))
			throw new DatabaseException("No insert happened");

		return item.Id;
	}
}