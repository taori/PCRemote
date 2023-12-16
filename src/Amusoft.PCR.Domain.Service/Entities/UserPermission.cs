using Amusoft.PCR.Domain.Service.ValueTypes;

namespace Amusoft.PCR.Domain.Service.Entities;

public class UserPermission
{
	public UserPermission(string userId, PermissionKind permissionType, string subjectId, bool granted)
	{
		UserId = userId;
		PermissionType = permissionType;
		SubjectId = subjectId;
		Granted = granted;
	}

	public UserPermission()
	{
	}

	public string UserId { get; init; }

	public PermissionKind PermissionType { get; init; }

	public string SubjectId { get; init; }

	public bool Granted { get; init; }
}