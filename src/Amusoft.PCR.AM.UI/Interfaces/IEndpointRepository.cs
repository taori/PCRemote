using System.Net;
using Amusoft.PCR.Domain.UI.Entities;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IEndpointRepository
{
	Task<Endpoint?> TryGetEndpointAsync(IPEndPoint endPoint, CancellationToken cancellationToken);
	Task<Endpoint> CreateEndpointAsync(IPEndPoint endPoint, CancellationToken cancellationToken);
	Task<EndpointAccount?> TryGetEndpointAccountAsync(Guid endPointId, string email, CancellationToken cancellationToken);
	Task<EndpointAccount> CreateEndpointAccountAsync(Guid endPointId, string email, CancellationToken cancellationToken);
	Task<EndpointAccount[]> GetEndpointAccountsAsync(IPEndPoint endPoint, CancellationToken cancellationToken);
	Task<EndpointAccount> GetEndpointAccountAsync(Guid endpointAccountId, CancellationToken cancellationToken);
	Task<int> RemoveEndpointAccountAsync(Guid endpointAccountId);
	Task<int> RemoveEndpointAsync(Guid endpointId);
}