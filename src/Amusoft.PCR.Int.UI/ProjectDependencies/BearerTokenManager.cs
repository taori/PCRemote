using System.Net;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Domain.UI.Entities;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.UI.ProjectDependencies;

internal class BearerTokenManager : IBearerTokenManager
{
	private readonly ILogger<BearerTokenManager> _logger;
	private readonly ICredentialPrompt _credentialPrompt;
	private readonly IBearerTokenRepository _tokenRepository;
	private readonly IIdentityManagerFactory _identityManagerFactory;
	private readonly IEndpointAccountManager _endpointAccountManager;

	public BearerTokenManager(
		ILogger<BearerTokenManager> logger
		, ICredentialPrompt credentialPrompt
		, IBearerTokenRepository tokenRepository
		, IIdentityManagerFactory identityManagerFactory
		, IEndpointAccountManager endpointAccountManager)
	{
		_logger = logger;
		_credentialPrompt = credentialPrompt;
		_tokenRepository = tokenRepository;
		_identityManagerFactory = identityManagerFactory;
		_endpointAccountManager = endpointAccountManager;
	}

	public async Task<string?> GetAccessTokenAsync(IPEndPoint endPoint, CancellationToken cancellationToken, string protocol)
	{
		if (await _endpointAccountManager.TryGetEndpointAccountAsync(endPoint, cancellationToken) is var endpointAccount && endpointAccount is null)
		{
			_logger.LogWarning("User declined account creation. Unable to continue.");
			return default;
		}

		var bearerToken = await _tokenRepository.GetLatestTokenAsync(endpointAccount.Id, cancellationToken);
		if (bearerToken is null)
			return default;

		if (DateTimeOffset.Now.AddSeconds(5) < bearerToken.Expires)
			return bearerToken.AccessToken;

		// token expires soon. time to refresh it
		var userManager = _identityManagerFactory.Create(endPoint, protocol);
		var refreshSignIn = await userManager.RefreshAsync(bearerToken.RefreshToken, cancellationToken);
		if (refreshSignIn is null)
		{
			_logger.LogDebug("Refresh failed. Requesting user password to login again.");
			var password = await _credentialPrompt.GetPasswordAsync(endpointAccount.Email);
			if (password is null)
			{
				_logger.LogError("Password prompt declined");
				return default;
			}

			refreshSignIn = await userManager.LoginAsync(endpointAccount.Email, password, cancellationToken);
			if (refreshSignIn is null)
			{
				_logger.LogError("Login attempt failed");
				return default;
			}
		}

		if (!await _tokenRepository.AddTokenAsync(SignInToToken(refreshSignIn, endpointAccount.Id), cancellationToken))
		{
			_logger.LogError("Failed to add token to storage");
			return null;
		}

		return refreshSignIn.AccessToken;
	}

	private BearerToken SignInToToken(SignInResponse newToken, Guid endpointAccountId)
	{
		return new BearerToken
		{
			Expires = newToken.ValidUntil
			, AccessToken = newToken.AccessToken
			, RefreshToken = newToken.RefreshToken
			, EndpointAccountId = endpointAccountId
			, IssuedAt = DateTimeOffset.Now
			,
		};
	}
}