using Amusoft.PCR.Domain.Service.Entities;
using Amusoft.PCR.Domain.Service.ValueTypes;

namespace Amusoft.PCR.Int.IPC.Extensions;

public static class UserManagementCommandReceiverExtensions
{
	public static IEnumerable<UserPermission> ToDomainItems(this IEnumerable<GetPermissionsItem> source)
	{
		return source.Select(d => new UserPermission(d.UserId, PermissionKind.HostCommand, d.SubjectId, true));
	}

	public static IEnumerable<GetPermissionsItem> ToGrpcItems(this IEnumerable<UserPermission> source)
	{
		return source.Select(d => new GetPermissionsItem() { SubjectId = d.SubjectId, UserId = d.UserId, });
	}
}