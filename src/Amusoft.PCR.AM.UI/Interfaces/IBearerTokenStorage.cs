using System.Net;
using Amusoft.PCR.Domain.UI.Entities;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IBearerTokenStorage
{
	Task<BearerToken?> GetLatestTokenAsync(Guid endpointAccountId, CancellationToken cancellationToken);
	Task<bool> AddTokenAsync(BearerToken token, CancellationToken cancellationToken);
	Task<bool> DeleteAsync(IPEndPoint endPoint, CancellationToken none);
}