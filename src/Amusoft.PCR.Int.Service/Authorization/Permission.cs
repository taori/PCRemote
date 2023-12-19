using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Amusoft.PCR.Domain.Shared.ValueTypes;

namespace Amusoft.PCR.Int.Service.Authorization;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class Permission
{
	[ForeignKey(nameof(UserId))]
	public ApplicationUser User { get; set; }

	[MaxLength(40)]
	public string UserId { get; set; }
		
	public PermissionKind PermissionType { get; set; }

	[MaxLength(40)]
	public string SubjectId { get; set; }
}