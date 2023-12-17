using System.Net;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Domain.UI.Entities;
using Amusoft.PCR.Int.Identity;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.UI.ProjectDependencies;

internal class UserAccountManager : IUserAccountManager
{
	private readonly ILogger<UserAccountManager> _logger;
	private readonly IdentityClient _client;

	public UserAccountManager(ILogger<UserAccountManager> logger, HttpClient httpClient, IPEndPoint endPoint, string protocol)
	{
		_logger = logger;
		_client = new IdentityClient(httpClient, $"{protocol}://{endPoint}/identity/");
	}

	public async Task<bool> IsAuthenticatedAsync(CancellationToken cancellationToken)
	{
		try
		{
			var result = await _client.GetHelloAsync(cancellationToken);
			return string.IsNullOrEmpty(result);
		}
		catch (ApiException e)
		{
			_logger.LogError(e, "API error");
			return false;
		}
	}

	public async Task<SignInResponse?> LoginAsync(string email, string password, CancellationToken cancellationToken, string? twoFactorCode = default, string? twoFactoryRecoveryCode = default)
	{
		try
		{
			var requestTime = DateTimeOffset.Now;
			_logger.LogDebug("User {Email} is signing in", email);
			var result = await _client.PostLoginAsync(false, false, new LoginRequest()
			{
				Email = email,
				Password = password,
				TwoFactorCode = twoFactorCode,
				TwoFactorRecoveryCode = twoFactoryRecoveryCode
			}, cancellationToken);

			return new SignInResponse(result.AccessToken!, result.RefreshToken!, requestTime.AddSeconds((double)result.ExpiresIn!));
		}
		catch (ApiException e)
		{
			_logger.LogError(e, "API error");
			return default;
		}
	}

	public async Task<SignInResponse?> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
	{
		try
		{
			var requestTime = DateTimeOffset.Now;
			_logger.LogDebug("Requesting refresh for refreshToken {Token}...", refreshToken[..10]);
			var result = await _client.PostRefreshAsync(new RefreshRequest()
			{
				RefreshToken = refreshToken
			}, cancellationToken);

			return new SignInResponse(result.AccessToken!, result.RefreshToken!, requestTime.AddSeconds((double)result.ExpiresIn!));
		}
		catch (ApiException e)
		{
			_logger.LogError(e, "API error");
			return default;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="email"></param>
	/// <param name="password"></param>
	/// <param name="cancellationToken"></param>
	/// <exception cref="ApiException"></exception>
	/// <returns></returns>
	public Task RegisterAsync(string email, string password, CancellationToken cancellationToken)
	{
		return _client.PostRegisterAsync(new RegisterRequest() { Email = email, Password = password }, cancellationToken);
	}
}