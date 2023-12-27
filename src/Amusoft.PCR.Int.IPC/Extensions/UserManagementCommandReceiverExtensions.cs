using Amusoft.PCR.Domain.Shared.Entities;
using Amusoft.PCR.Domain.Shared.ValueTypes;
using DomainUserRole = Amusoft.PCR.Domain.Shared.Entities.UserRole;
using DomainUserPermission = Amusoft.PCR.Domain.Shared.Entities.UserPermission;

namespace Amusoft.PCR.Int.IPC.Extensions;

public static class UserManagementCommandReceiverExtensions
{
	public static IEnumerable<DomainUserRole> ToDomainItems(this IEnumerable<UserRole> source)
	{
		return source.Select(d => new DomainUserRole(d.RoleId, d.RoleName, d.Granted));
	}

	public static IEnumerable<UserRole> ToGrpcItems(this IEnumerable<DomainUserRole> source)
	{
		return source.Select(d => new UserRole()
		{
			RoleId = d.Id
			, RoleName = d.Name
			, Granted = d.Granted
		});
	}

	public static IEnumerable<DomainUserPermission> ToDomainItems(this IEnumerable<UserPermission> source)
	{
		return source.Select(d => new DomainUserPermission((PermissionKind)d.PermissionType, d.SubjectId, d.Name, d.Granted));
	}

	public static IEnumerable<UserPermission> ToGrpcItems(this IEnumerable<DomainUserPermission> source)
	{
		return source.Select(d => new UserPermission()
		{
			SubjectId = d.SubjectId
			, Granted = d.Granted
			, PermissionType = (int)d.PermissionType
			, Name = d.Name
		});
	}

	public static IEnumerable<GetRegisteredUsersResponseItem> ToGrpcItems(this IEnumerable<RegisteredUser> source)
	{
		return source.Select(
			d => new GetRegisteredUsersResponseItem()
			{
				Address = d.Email,
				Id = d.Id,
			}
		);
	}

	public static IEnumerable<RegisteredUser> ToDomainItems(this IEnumerable<GetRegisteredUsersResponseItem> source)
	{
		return source.Select(
			d => new RegisteredUser()
			{
				Email = d.Address,
				Id = d.Id,
			}
		);
	}
}