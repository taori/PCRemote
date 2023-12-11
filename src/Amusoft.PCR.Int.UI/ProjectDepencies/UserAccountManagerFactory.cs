using System.Net;
using Amusoft.PCR.AM.UI.Interfaces;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.UI.ProjectDepencies;

internal class UserAccountManagerFactory : IUserAccountManagerFactory
{
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly ILogger<UserAccountManager> _logger;

	public UserAccountManagerFactory(IHttpClientFactory httpClientFactory, ILogger<UserAccountManager> logger)
	{
		_httpClientFactory = httpClientFactory;
		_logger = logger;
	}

	public IUserAccountManager Create(IPEndPoint endPoint, string protocol)
	{
		return new UserAccountManager(_logger, _httpClientFactory.CreateClient(endPoint.ToString().ToLowerInvariant()), endPoint, protocol);
	}
}