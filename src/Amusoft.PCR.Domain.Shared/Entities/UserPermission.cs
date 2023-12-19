using System.Diagnostics.CodeAnalysis;
using Amusoft.PCR.Domain.Shared.ValueTypes;

namespace Amusoft.PCR.Domain.Shared.Entities;

public class UserPermission
{
	[SetsRequiredMembers]
	public UserPermission(PermissionKind permissionType, string subjectId, string name, bool granted)
	{
		PermissionType = permissionType;
		SubjectId = subjectId;
		Name = name;
		Granted = granted;
	}

	public UserPermission()
	{
	}

	public required PermissionKind PermissionType { get; init; }

	public required string SubjectId { get; init; }

	public required string Name { get; init; }

	public required bool Granted { get; init; }
}