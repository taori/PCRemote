#region

using System.Net;
using Amusoft.PCR.Domain.UI.Entities;

#endregion

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IBearerTokenStorage
{
	Task<BearerToken?> GetTokenAsync(IPEndPoint endPoint, CancellationToken cancellationToken);
	Task<bool> AddTokenAsync(IPEndPoint endPoint, BearerToken token, CancellationToken cancellationToken);
}