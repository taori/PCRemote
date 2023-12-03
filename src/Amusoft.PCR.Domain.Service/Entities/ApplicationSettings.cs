namespace Amusoft.PCR.Domain.Service.Entities;


public class ApplicationSettings
{
	public string? ApplicationTitle { get; set; }
	public bool DropDatabaseOnStart { get; set; }
	public DesktopIntegrationSettings? DesktopIntegration { get; set; }
	public ServerUrlTransmitterSettings? ServerUrlTransmitter { get; set; }
	public AuthenticationSettings? Authentication { get; set; }
	public JwtSettings? Jwt { get; set; }
}