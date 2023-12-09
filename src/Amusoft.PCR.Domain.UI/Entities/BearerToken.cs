#region

using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace Amusoft.PCR.Domain.UI.Entities;

public class BearerToken
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public required Guid Id { get; set; }

	public required string Address { get; set; }

	public required string AccessToken { get; set; }

	public required string RefreshToken { get; set; }

	public required DateTimeOffset Expires { get; set; }
}