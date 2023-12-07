using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using Amusoft.PCR.Domain.Service.Entities;
using Amusoft.PCR.Domain.Service.Entities.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Amusoft.PCR.Int.Service.Authorization;

public interface IJwtTokenService
{
	Task<JwtAuthenticationResult> CreateAuthenticationAsync(string userName, string password);
	Task<JwtAuthenticationResult> RefreshAsync(string expiredAccessToken, string refreshToken);
	bool TryGetGetUserFromToken(string token, out string userName, out SecurityToken securityToken);
}

public class JwtTokenService : IJwtTokenService
{
	private readonly IRefreshTokenManager _refreshTokenManager;
	private readonly TokenValidationParameters _tokenValidationParameters;
	private readonly ILogger<JwtTokenService> _logger;
	private readonly JwtSettings _options;
	private readonly UserManager<ApplicationUser> _userManager;

	public JwtTokenService(
		IRefreshTokenManager refreshTokenManager,
		TokenValidationParameters tokenValidationParameters,
		ILogger<JwtTokenService> logger,
		IOptions<ApplicationSettings> options,
		UserManager<ApplicationUser> userManager)
	{
		_refreshTokenManager = refreshTokenManager;
		_tokenValidationParameters = tokenValidationParameters;
		_logger = logger;
		_options = options.Value.Jwt;
		_userManager = userManager;
	}

	public async Task<JwtAuthenticationResult> CreateAuthenticationAsync(string userName, string password)
	{
		_logger.LogInformation("Authenticating user {User}", userName);
		var user = await _userManager.FindByNameAsync(userName);
		if (!await _userManager.CheckPasswordAsync(user, password))
		{
			_logger.LogWarning("Failed login attempt for {User}", userName);
			await _userManager.AccessFailedAsync(user);
			return new JwtAuthenticationResult() { InvalidCredentials = true };
		}

		return await CreateAuthenticationResultFromUserAsync(user);
	}

	private async Task<JwtAuthenticationResult> CreateAuthenticationResultFromUserAsync(ApplicationUser user)
	{
		if (string.IsNullOrEmpty(user.UserName))
			throw new Exception("The Name of a user instance cannot be empty");
		
		var roles = await _userManager.GetRolesAsync(user);
		var handler = new JwtSecurityTokenHandler();
		var claims = new List<Claim>();
		claims.Add(new Claim(ClaimTypes.Name, user.UserName));
		claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
		claims.Add(new Claim(JwtRegisteredClaimNames.Aud, _tokenValidationParameters.ValidAudience));
		claims.AddRange(roles.Select(d => new Claim(ClaimTypes.Role, d)));

		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
		var securityToken = new JwtSecurityToken(
			_options.Issuer,
			claims: claims,
			signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512),
			expires: DateTime.UtcNow.Add(_options.AccessTokenValidDuration));

		var outputToken = handler.WriteToken(securityToken);
		var refreshToken = GenerateRefreshToken();

		await _refreshTokenManager.AddRefreshTokenAsync(user, refreshToken, DateTime.UtcNow.Add(_options.RefreshTokenValidDuration));

		return new JwtAuthenticationResult()
		{
			AccessToken = outputToken,
			RefreshToken = refreshToken
		};
	}

	public async Task<JwtAuthenticationResult> RefreshAsync(string expiredAccessToken, string refreshToken)
	{
		_logger.LogTrace("Reading username from token {Token}", refreshToken);
		if (!TryGetGetUserFromToken(expiredAccessToken, out var userName, out var securityToken))
		{
			_logger.LogWarning("Failed to get user through token");
			return new JwtAuthenticationResult() { AuthenticationRequired = true };
		}

		_logger.LogTrace("Obtaining user through userName {@UserName}", userName);
		var user = await _userManager.FindByNameAsync(userName);
		if (user == null)
		{
			_logger.LogWarning("Failed to get user through principal");
			return new JwtAuthenticationResult() { AuthenticationRequired = true };
		}

		var refreshTokenData = await _refreshTokenManager.GetRefreshTokenAsync(user, refreshToken);
		if (refreshTokenData == null)
		{
			_logger.LogWarning("No refresh token available but there should be one");
			return new JwtAuthenticationResult() { AuthenticationRequired = true };
		}
		else
		{
			if (refreshTokenData.IsUsed)
			{
				_logger.LogWarning("A used token refresh token was sent again - perhaps someone stole the token, expire all client tokens and force re-authentication");
				await _refreshTokenManager.RemoveAllAsync(user);
				return new JwtAuthenticationResult() { AuthenticationRequired = true };
			}
		}

		using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
		_logger.LogDebug("Generating new authentication from refresh token");
		var authenticationResult = await CreateAuthenticationResultFromUserAsync(user);

		await _refreshTokenManager.InvalidateRefreshTokenAsync(user, refreshToken);

		transaction.Complete();

		return authenticationResult;
	}

	public string GenerateRefreshToken()
	{
		var randomNumber = new byte[32];
		using (var generator = RandomNumberGenerator.Create())
		{
			generator.GetBytes(randomNumber);
			return Convert.ToBase64String(randomNumber);
		}
	}

	public bool TryGetGetUserFromToken(string token, out string userName, out SecurityToken securityToken)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var tokenValidationParameters = new TokenValidationParameters();
		tokenValidationParameters.ValidateIssuer = true;
		tokenValidationParameters.ValidateAudience = true;
		tokenValidationParameters.ValidateLifetime = false;
		tokenValidationParameters.ValidateIssuerSigningKey = true;
		tokenValidationParameters.ValidIssuer = _options.Issuer;
		tokenValidationParameters.ValidAudience = _options.Issuer;
		tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
		var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

		userName = principal.Identity?.Name;
		if (userName == null)
			return false;

		return true;
	}
}