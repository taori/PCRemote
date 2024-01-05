#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace Amusoft.PCR.Domain.Service.Entities;


public class ApplicationSettings
{
	public string ApplicationTitle { get; set; }
	public bool DropDatabaseOnStart { get; set; }
	public bool RuntimePermissionRequests { get; set; }
	public DesktopIntegrationSettings? DesktopIntegration { get; set; }
	public ServerUrlTransmitterSettings? ServerUrlTransmitter { get; set; }
	public AuthenticationSettings? Authentication { get; set; }
	public JwtSettings? Jwt { get; set; }
}