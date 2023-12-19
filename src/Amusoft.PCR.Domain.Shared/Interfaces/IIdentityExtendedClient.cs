using Amusoft.PCR.Domain.Shared.Entities;

namespace Amusoft.PCR.Domain.Shared.Interfaces;

public interface IIdentityExtendedClient
{
	Task<bool?> RequestAdminToggle(string email, CancellationToken cancellationToken);
	Task<UserPermissionSet?> GetPermissionsAsync(string email, CancellationToken cancellationToken);
	Task<bool?> UpdatePermissionsAsync(string email, UserPermissionSet permissions, CancellationToken cancellationToken);
}