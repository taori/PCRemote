using System.Net;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IEndpointAccountSelection
{
	Task<(string? mail, Guid? endpointAccountId)> GetCurrentAccountOrPromptAsync(IPEndPoint endPoint, CancellationToken cancellationToken);
	Task<Guid?> GetCurrentAccountAsync(IPEndPoint endPoint, CancellationToken cancellationToken);
	Task SetEndpointAccountAsync(IPEndPoint endPoint, Guid endpointAccountId, CancellationToken cancellationToken);
}