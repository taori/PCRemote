using System.Net;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IBearerTokenProvider
{
	Task<string?> GetAccessTokenAsync(IPEndPoint endPoint, CancellationToken cancellationToken, string protocol);
}