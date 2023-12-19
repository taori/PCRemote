using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Domain.UI.Entities;

namespace Amusoft.PCR.Int.UI.ProjectDependencies;

internal class EndpointTokenBroker : IEndpointTokenBroker
{
	private readonly BearerToken _token;
	private readonly Guid _endpointAccountId;
	private readonly IIdentityManager _identityManager;
	private readonly IBearerTokenRepository _bearerTokenRepository;

	public EndpointTokenBroker(BearerToken token, Guid endpointAccountId, IIdentityManager identityManager, IBearerTokenRepository bearerTokenRepository)
	{
		_token = token;
		_endpointAccountId = endpointAccountId;
		_identityManager = identityManager;
		_bearerTokenRepository = bearerTokenRepository;
	}

	public async Task<string?> GetAccessTokenAsync()
	{
		if (DateTimeOffset.Now.AddSeconds(10) > _token.Expires)
		{
			var update = await _identityManager.RefreshAsync(_token.RefreshToken, CancellationToken.None).ConfigureAwait(false);
			if (update is null)
				return default;

			var bearerToken = new BearerToken(update.AccessToken, update.RefreshToken, update.ValidUntil, DateTimeOffset.Now, _endpointAccountId);
			if (!await _bearerTokenRepository.AddTokenAsync(bearerToken, CancellationToken.None).ConfigureAwait(false))
				return default;

			return update.AccessToken;
		}

		return _token.AccessToken;
	}
}