using System.Net;
using Amusoft.PCR.Domain.UI.Entities;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IEndpointAccountManager
{
	Task<EndpointAccount?> TryGetEndpointAccountAsync(IPEndPoint endPoint, CancellationToken cancellationToken);
	Task AddBearerTokenAsync(BearerToken bearerToken);
}