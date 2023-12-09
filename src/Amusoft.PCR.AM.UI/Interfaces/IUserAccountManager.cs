#region

using Amusoft.PCR.Domain.UI.Entities;

#endregion

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IUserAccountManager
{
	Task<bool> IsAuthenticatedAsync(CancellationToken cancellationToken);
	Task<SignInResponse?> LoginAsync(string email, string password, CancellationToken cancellationToken, string? twoFactorCode = default, string? twoFactoryRecoveryCode = default);
	Task<SignInResponse?> RefreshAsync(string refreshToken, CancellationToken cancellationToken);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="email"></param>
	/// <param name="password"></param>
	/// <param name="cancellationToken"></param>
	/// <exception cref="Amusoft.PCR.Int.Identity.ApiException"></exception>
	/// <returns></returns>
	Task RegisterAsync(string email, string password, CancellationToken cancellationToken);
}