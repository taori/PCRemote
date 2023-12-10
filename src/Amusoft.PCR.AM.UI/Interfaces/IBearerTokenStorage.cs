using System.Net;
using Amusoft.PCR.Domain.UI.Entities;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IBearerTokenStorage
{
	Task<BearerToken?> GetLatestTokenAsync(IPEndPoint endPoint, CancellationToken cancellationToken);
	Task<bool> AddTokenAsync(IPEndPoint endPoint, BearerToken token, CancellationToken cancellationToken);
	Task<bool> PruneAsync(IPEndPoint ipEndPoint, CancellationToken none);
}