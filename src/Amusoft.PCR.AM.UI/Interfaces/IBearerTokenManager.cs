using System.Net;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IBearerTokenManager
{
	Task<string?> GetAccessTokenAsync(IPEndPoint endPoint, CancellationToken cancellationToken, string protocol);
}