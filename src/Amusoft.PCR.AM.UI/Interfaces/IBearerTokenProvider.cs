#region

using System.Net;

#endregion

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IBearerTokenProvider
{
	Task<string?> GetAccessTokenAsync(IPEndPoint endPoint, CancellationToken cancellationToken);
}