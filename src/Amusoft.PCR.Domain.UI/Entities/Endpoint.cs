using System.ComponentModel.DataAnnotations.Schema;

namespace Amusoft.PCR.Domain.UI.Entities;

public class Endpoint
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public Guid Id { get; set; }

	public required string Address { get; set; }

	public ICollection<EndpointAccount> Accounts { get; set; }
}