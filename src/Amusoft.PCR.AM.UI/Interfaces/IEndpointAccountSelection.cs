using System.Net;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IEndpointAccountSelection
{
	Task<(string? mail, Guid? endpointAccountId)> GetCurrentAccountOrPromptAsync(IPEndPoint endPoint);
	Task<Guid?> GetCurrentAccountAsync(IPEndPoint endPoint);
	Task SetEndpointAccountAsync(IPEndPoint endPoint, Guid endpointAccountId);
}