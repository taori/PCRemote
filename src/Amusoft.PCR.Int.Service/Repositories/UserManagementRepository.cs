using Amusoft.PCR.AM.Service.Interfaces;
using Amusoft.PCR.Domain.Service.Entities;
using Amusoft.PCR.Domain.Shared.Entities;
using Amusoft.PCR.Domain.Shared.ValueTypes;
using Amusoft.PCR.Int.Service.Authorization;
using Amusoft.PCR.Int.Service.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.Service.Repositories;

public class UserManagementRepository : IUserManagementRepository
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly ILogger<UserManagementRepository> _logger;
	private readonly ApplicationDbContext _applicationDbContext;

	public UserManagementRepository(
		UserManager<ApplicationUser> userManager
		, RoleManager<IdentityRole> roleManager
		, ILogger<UserManagementRepository> logger
		, ApplicationDbContext applicationDbContext)
	{
		_userManager = userManager;
		_roleManager = roleManager;
		_logger = logger;
		_applicationDbContext = applicationDbContext;
	}

	public async Task<UserPermissionSet?> GetPermissionsAsync(string email, CancellationToken cancellationToken)
	{
		var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
		if (user is null)
			return default;

		var allRoles = await _roleManager.Roles.ToArrayAsync(cancellationToken).ConfigureAwait(false);
		var userRoles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
		var userRolesGranted = userRoles.ToHashSet();
		// todo appdbconcontext Permissions

		return new UserPermissionSet()
		{
			Roles = allRoles
				.Select(d => new UserRole(d.Id, d.Name!, userRolesGranted.Contains(d.NormalizedName!)))
				.ToArray()
			, UserId = user.Id
			, UserType = user.UserType
			, Permissions = Array.Empty<UserPermission>()
		};
	}

	public async Task<bool> SetUserTypeAsync(string email, UserType newUserType, CancellationToken cancellationToken)
	{
		if (await TryFindUserByMailAsync(email) is var user && user is null)
			return false;

		if (newUserType == UserType.Administrator)
			await _userManager.AddToRoleAsync(user, RoleNames.Administrator);
		else
			await _userManager.RemoveFromRoleAsync(user, RoleNames.Administrator);

		user.UserType = newUserType;

		if (await _userManager.UpdateAsync(user) is { Succeeded: false } updateUser)
		{
			_logger.LogError(updateUser.ToLogMessage());
			return false;
		}

		return true;
	}

	public async Task<bool> UpdatePermissionsAsync(string email, UserPermissionSet permissionSet, CancellationToken cancellationToken)
	{
		if (await TryFindUserByMailAsync(email) is var user && user is null)
			return false;

		var allRoles = _roleManager.Roles.ToArray();
		var normalizedNameById = allRoles.ToDictionary(d => d.Id, d => d.NormalizedName!);
		var roleIdByName = allRoles.ToDictionary(d => d.NormalizedName!, d => d.Id);
		var existingRoleIds = (await _userManager.GetRolesAsync(user)).Select(name => roleIdByName[name]).ToHashSet(StringComparer.OrdinalIgnoreCase);
		var desired = permissionSet.Roles.Where(d => d.Granted);
		var undesired = permissionSet.Roles.Where(d => !d.Granted);
		var grants = desired.Where(d => !existingRoleIds.Contains(d.Id));
		var revokes = undesired.Where(d => existingRoleIds.Contains(d.Id));

		var roleRemoval = await _userManager.RemoveFromRolesAsync(user, revokes.Select(d => normalizedNameById[d.Id]));
		var roleAddition = await _userManager.AddToRolesAsync(user, grants.Select(d => normalizedNameById[d.Id]));
		if (!roleRemoval.Succeeded)
			_logger.LogError(roleRemoval.ToLogMessage());
		if (!roleAddition.Succeeded)
			_logger.LogError(roleAddition.ToLogMessage());

		// todo appdbconcontext Permissions

		return roleAddition.Succeeded && roleRemoval.Succeeded;
	}

	public async Task<UserType> GetUserTypeAsync(string email)
	{
		var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
		if (user is null)
		{
			_logger.LogError("Email {Email} not found", email);
			return default;
		}

		return user.UserType;
	}

	private async Task<ApplicationUser?> TryFindUserByMailAsync(string email)
	{
		var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
		if (user is null)
		{
			_logger.LogError("Email {Email} not found", email);
			return default;
		}

		return user;
	}
}