using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Amusoft.PCR.Int.Service.Authorization;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class RefreshToken
{
	[ForeignKey(nameof(UserId))]
	public ApplicationUser User { get; set; }
	public string UserId { get; set; }

	[MaxLength(50)]
	public string RefreshTokenId { get; set; }

	public bool IsUsed { get; set; }

	public DateTime ValidUntil { get; set; }

	public DateTime IssuedAt { get; set; }
}