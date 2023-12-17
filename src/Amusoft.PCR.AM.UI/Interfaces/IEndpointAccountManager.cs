using System.Net;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IEndpointAccountManager
{
	Task<Guid?> GetEndpointAccountIdAsync(IPEndPoint endPoint);
}