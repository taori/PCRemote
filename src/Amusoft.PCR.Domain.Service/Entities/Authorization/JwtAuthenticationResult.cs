namespace Amusoft.PCR.Domain.Service.Entities.Authorization;

public class JwtAuthenticationResult
{
	public bool AuthenticationRequired { get; set; }

	public bool InvalidCredentials { get; set; }

	public string AccessToken { get; set; }

	public string RefreshToken { get; set; }
}