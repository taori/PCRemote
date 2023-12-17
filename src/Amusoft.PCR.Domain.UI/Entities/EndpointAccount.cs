using System.ComponentModel.DataAnnotations.Schema;

namespace Amusoft.PCR.Domain.UI.Entities;

public class EndpointAccount
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public Guid Id { get; set; }

	public required Guid EndpointId { get; set; }

	public Endpoint Endpoint { get; set; }

	public required string Email { get; set; }

	public ICollection<BearerToken> BearerTokens { get; set; }
}