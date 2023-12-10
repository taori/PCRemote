#region

using System.Net;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Domain.UI.Entities;
using Microsoft.Extensions.Logging;

#endregion

namespace Amusoft.PCR.Int.UI.ProjectDepencies;

internal class BearerTokenProvider : IBearerTokenProvider
{
	private readonly ILogger<BearerTokenProvider> _logger;
	private readonly IBearerTokenStorage _storage;
	private readonly ICredentialUserPrompt _credentialUserPrompt;
	private readonly IUserAccountManagerFactory _userAccountManagerFactory;

	public BearerTokenProvider(ILogger<BearerTokenProvider> logger, IBearerTokenStorage storage, ICredentialUserPrompt credentialUserPrompt, IUserAccountManagerFactory userAccountManagerFactory)
	{
		_logger = logger;
		_storage = storage;
		_credentialUserPrompt = credentialUserPrompt;
		_userAccountManagerFactory = userAccountManagerFactory;
	}

	public async Task<string?> GetAccessTokenAsync(IPEndPoint endPoint, CancellationToken cancellationToken)
	{
		var token = await _storage.GetTokenAsync(endPoint, cancellationToken);
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
				var userManager = _userAccountManagerFactory.Create(endPoint);
				if (await userManager.LoginAsync(credentials.Value.email, credentials.Value.password, cancellationToken) is { } newToken)
				{
					if (await AddTokenToStorage(endPoint, cancellationToken, newToken))
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
				var userManager = _userAccountManagerFactory.Create(endPoint);
				if (await userManager.RefreshAsync(token.RefreshToken, cancellationToken) is { } newToken)
				{
					if (await AddTokenToStorage(endPoint, cancellationToken, newToken))
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

	private Task<bool> AddTokenToStorage(IPEndPoint endPoint, CancellationToken cancellationToken, SignInResponse newToken)
	{
		return _storage.AddTokenAsync(endPoint, SignInToToken(newToken, endPoint), cancellationToken);
	}

	private BearerToken SignInToToken(SignInResponse newToken, IPEndPoint ipEndPoint)
	{
		return new BearerToken()
		{
			Address = ipEndPoint.ToString(),
			Expires = newToken.ValidUntil,
			AccessToken = newToken.AccessToken,
			RefreshToken = newToken.RefreshToken,
		};
	}
}