using Amusoft.PCR.Domain.Shared.ValueTypes;

namespace Amusoft.PCR.Domain.Shared.Entities;

public class UserPermissionSet
{
	public required string UserId { get; set; }

	public UserType UserType { get; set; }

	public required UserPermission[] Permissions { get; set; } = Array.Empty<UserPermission>();

	public required UserRole[] Roles { get; set; } = Array.Empty<UserRole>();
}