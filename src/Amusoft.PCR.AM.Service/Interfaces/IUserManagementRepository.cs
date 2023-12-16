using Amusoft.PCR.Domain.Service.Entities;

namespace Amusoft.PCR.AM.Service.Interfaces;

public interface IUserManagementRepository
{
	Task<UserPermission[]> GetPermissionsAsync(string email, CancellationToken cancellationToken);
	Task<bool> SetUserTypeAdminAsync(string email, CancellationToken cancellationToken);
	Task<bool> UpdatePermissionsAsync(string email, IEnumerable<UserPermission> items, CancellationToken cancellationToken);
}