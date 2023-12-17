using System.Net;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Domain.UI.Entities;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.UI.ProjectDependencies;

internal class BearerTokenManager : IBearerTokenManager
{
	private readonly ILogger<BearerTokenManager> _logger;
	private readonly IBearerTokenRepository _repository;
	private readonly ICredentialUserPrompt _credentialUserPrompt;
	private readonly IIdentityManagerFactory _identityManagerFactory;
	private readonly IEndpointAccountManager _endpointAccountManager;

	public BearerTokenManager(
		ILogger<BearerTokenManager> logger
		, IBearerTokenRepository repository
		, ICredentialUserPrompt credentialUserPrompt
		, IIdentityManagerFactory identityManagerFactory
		, IEndpointAccountManager endpointAccountManager)
	{
		_logger = logger;
		_repository = repository;
		_credentialUserPrompt = credentialUserPrompt;
		_identityManagerFactory = identityManagerFactory;
		_endpointAccountManager = endpointAccountManager;
	}

	public async Task<string?> GetAccessTokenAsync(IPEndPoint endPoint, CancellationToken cancellationToken, string protocol)
	{
		if (await _endpointAccountManager.GetEndpointAccountIdAsync(endPoint) is var endpointAccountId && endpointAccountId is null)
		{
			_logger.LogWarning("User declined account creation. Unable to continue.");
			return null;
		}

		var token = await _repository.GetLatestTokenAsync(endpointAccountId.Value, cancellationToken);
		if (token is null)
		{
			var credentials = await _credentialUserPrompt.SignInAsync();
			if (credentials is null)
			{
				_logger.LogWarning("User provided no credentials. Unable to continue.");
				return null;
			}
			else
			{
				var identityManager = _identityManagerFactory.Create(endPoint, protocol);
				if (await identityManager.LoginAsync(credentials.Value.email, credentials.Value.password, cancellationToken) is { } newToken)
				{
					if (await AddTokenToStorage(endpointAccountId.Value, cancellationToken, newToken))
					{
						return newToken.AccessToken;
					}

					_logger.LogError("Failed to add token to storage");
				}
				else
				{
					_logger.LogError("Login failed");
				}
			}
		}
		else
		{
			if (token.Expires.AddSeconds(-5) <= DateTimeOffset.Now)
			{
				// token expires soon. time to refresh it
				var userManager = _identityManagerFactory.Create(endPoint, protocol);
				if (await userManager.RefreshAsync(token.RefreshToken, cancellationToken) is { } newToken)
				{
					if (await AddTokenToStorage(endpointAccountId.Value, cancellationToken, newToken))
					{
						return newToken.AccessToken;
					}

					_logger.LogError("Failed to add token to storage");
				}
				else
				{
					_logger.LogError("Token refresh failed");
				}
			}
			else
			{
				return token.AccessToken;
			}
		}

		return null;
	}

	private Task<bool> AddTokenToStorage(Guid endpointAccountId, CancellationToken cancellationToken, SignInResponse newToken)
	{
		return _repository.AddTokenAsync(SignInToToken(newToken, endpointAccountId), cancellationToken);
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