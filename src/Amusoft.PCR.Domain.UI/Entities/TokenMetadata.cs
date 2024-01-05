namespace Amusoft.PCR.Domain.UI.Entities;

public class TokenMetadata
{
	public string UserId { get; set; }

	public DateTimeOffset Expires { get; set; }
}