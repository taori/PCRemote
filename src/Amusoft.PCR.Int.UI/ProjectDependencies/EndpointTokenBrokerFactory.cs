using Amusoft.PCR.AM.UI.Interfaces;

namespace Amusoft.PCR.Int.UI.ProjectDependencies;

internal class EndpointTokenBrokerFactory : IEndpointTokenBrokerFactory
{
	private readonly IBearerTokenRepository _bearerTokenRepository;
	private readonly IIdentityManagerFactory _identityManagerFactory;
	private readonly IHostCredentials _hostCredentials;

	public EndpointTokenBrokerFactory(IBearerTokenRepository bearerTokenRepository, IIdentityManagerFactory identityManagerFactory, IHostCredentials hostCredentials)
	{
		_bearerTokenRepository = bearerTokenRepository;
		_identityManagerFactory = identityManagerFactory;
		_hostCredentials = hostCredentials;
	}

	public async Task<IEndpointTokenBroker> CreateAsync(Guid endpointAccountId)
	{
		var token = await _bearerTokenRepository.GetLatestTokenAsync(endpointAccountId, CancellationToken.None);
		if (token is null)
			throw new Exception($"No latest bearer token available for endpointAccountId {endpointAccountId}");

		return new EndpointTokenBroker(token, endpointAccountId, _identityManagerFactory.Create(_hostCredentials.Address, _hostCredentials.Protocol), _bearerTokenRepository);
	}
}