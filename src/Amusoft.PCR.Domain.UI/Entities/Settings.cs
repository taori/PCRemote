using System.Net;

namespace Amusoft.PCR.Domain.UI.Entities;

public class Settings
{
	public int? Sensitivity { get; set; }
	
	public int[] Ports { get; set; } = Array.Empty<int>();

	public Dictionary<IPEndPoint, Guid> EndpointAccountIdByEndpoint { get; set; } = new();

	public LogSettings LogSettings { get; set; } = new();
}