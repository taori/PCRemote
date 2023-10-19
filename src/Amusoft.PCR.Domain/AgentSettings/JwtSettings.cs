namespace Amusoft.PCR.Domain.AgentSettings;

public class JwtSettings
{
	public TimeSpan RefreshAccessTokenInterval { get; set; }
	public TimeSpan RefreshTokenValidDuration { get; set; }
	public TimeSpan AccessTokenValidDuration { get; set; }
	public string? Key { get; set; }
	public string? Issuer { get; set; }
}