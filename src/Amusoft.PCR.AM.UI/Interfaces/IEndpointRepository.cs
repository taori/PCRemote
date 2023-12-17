using System.Net;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IEndpointRepository
{
	Task<Guid?> GetEndpointIdAsync(IPEndPoint endPoint);
	Task<Guid> CreateEndpointAsync(IPEndPoint endPoint);
	Task<Guid?> GetEndpointAccountIdAsync(Guid endPointId, string email);
	Task<Guid> CreateEndpointAccountAsync(Guid endPointId, string email);
}