﻿using Amusoft.PCR.Domain.Shared.Entities;
using Amusoft.PCR.Domain.Shared.ValueTypes;

namespace Amusoft.PCR.AM.Service.Interfaces;

public interface IUserManagementRepository
{
	Task<UserPermissionSet?> GetPermissionsAsync(string email, CancellationToken cancellationToken);
	Task<bool> SetUserTypeAsync(string email, UserType newUserType, CancellationToken cancellationToken);
	Task<bool> UpdatePermissionsAsync(string email, UserPermissionSet permissionSet, CancellationToken cancellationToken);
	Task<UserType> GetUserTypeAsync(string email);
}