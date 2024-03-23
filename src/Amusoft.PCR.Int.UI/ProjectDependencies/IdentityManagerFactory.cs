using System.Net;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Domain.Shared.Entities;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.UI.ProjectDependencies;

internal class IdentityManagerFactory : IIdentityManagerFactory
{
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly ILogger<IdentityManager> _logger;

	public IdentityManagerFactory(IHttpClientFactory httpClientFactory, ILogger<IdentityManager> logger)
	{
		_httpClientFactory = httpClientFactory;
		_logger = logger;
	}

	public IIdentityManager Create(IPEndPoint endPoint, string protocol)
	{
		return new IdentityManager(_logger, _httpClientFactory.CreateClient(HttpClientNames.Insecure), endPoint, protocol);
		// return new IdentityManager(_logger, _httpClientFactory.CreateClient(endPoint.ToString().ToLowerInvariant()), endPoint, protocol);
	}
}