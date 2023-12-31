﻿using Amusoft.PCR.AM.Service.Interfaces;
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
	private readonly ApplicationDbContext _applicationDbContext;
	private readonly ILogger<UserManagementRepository> _logger;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly UserManager<ApplicationUser> _userManager;

	public UserManagementRepository(
		UserManager<ApplicationUser> userManager,
		RoleManager<IdentityRole> roleManager,
		ILogger<UserManagementRepository> logger,
		ApplicationDbContext applicationDbContext)
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
		var userRolesGranted = userRoles.ToHashSet(StringComparer.OrdinalIgnoreCase);
		// todo appdbconcontext Permissions

		var result = new UserPermissionSet()
		{
			Roles = allRoles
				.Select(d => new UserRole(d.Id, d.Name!, userRolesGranted.Contains(d.NormalizedName!)))
				.ToArray(),
			UserId = user.Id, UserType = user.UserType, Permissions = Array.Empty<UserPermission>()
		};

		return result;
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
		var userRoles = (await _userManager.GetRolesAsync(user)).Select(d => d.ToUpperInvariant()).ToArray();
		var existingRoleIds = userRoles.Select(name => roleIdByName[name]).ToHashSet(StringComparer.OrdinalIgnoreCase);
		var desired = permissionSet.Roles.Where(d => d.Granted);
		var undesired = permissionSet.Roles.Where(d => !d.Granted);
		var grants = desired.Where(d => !existingRoleIds.Contains(d.Id)).ToArray();
		var revokes = undesired.Where(d => existingRoleIds.Contains(d.Id)).ToArray();

		bool revoke = false, grant = false;
		if (revokes.Length > 0 && await _userManager.RemoveFromRolesAsync(user, revokes.Select(d => normalizedNameById[d.Id])) is { } revokeAction)
		{
			if (!revokeAction.Succeeded)
				_logger.LogError(revokeAction.ToLogMessage());
			revoke = revokeAction.Succeeded;
		}

		if (grants.Length > 0 && await _userManager.AddToRolesAsync(user, grants.Select(d => normalizedNameById[d.Id])) is { } additionAction)
		{
			if (!additionAction.Succeeded)
				_logger.LogError(additionAction.ToLogMessage());
			grant = additionAction.Succeeded;
		}

		// todo appdbconcontext Permissions

		return revoke && grant;
	}

	public async Task<UserType> GetUserTypeAsync(string email, CancellationToken cancellationToken)
	{
		var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
		if (user is null)
		{
			_logger.LogError("Email {Email} not found", email);
			return default;
		}

		return user.UserType;
	}

	public Task<RegisteredUser[]> GetUsersAsync(CancellationToken cancellationToken)
	{
		var users = _userManager.Users;
		return Task.FromResult(
			users.Select(
				d => new RegisteredUser()
				{
					Email = d.Email,
					Id = d.Id.ToString()
				}
			).ToArray()
		);
	}

	public async Task<bool> DeleteUserAsync(string email)
	{
		var user = await _userManager.FindByEmailAsync(email);
		if (user != null)
		{
			var deletion = await _userManager.DeleteAsync(user);
			if (!deletion.Succeeded)
			{
				_logger.LogError("Deletion failed: {Message}", deletion.ToLogMessage());
				return false;
			}

			return true;
		}
		else
		{
			_logger.LogError("Deletion failed, user not found");
			return false;
		}
	}

	public async Task<bool> GrantRolesAsync(string email, ICollection<string> roles)
	{
		var user = await _userManager.FindByEmailAsync(email);
		if (user is null)
			return false;
		var allRoles = _roleManager.Roles
			.Select(d => d.NormalizedName)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var currentRoles = await _userManager.GetRolesAsync(user);
		var intersect = roles.Intersect(allRoles, StringComparer.OrdinalIgnoreCase);
		var others = intersect.Except(currentRoles, StringComparer.OrdinalIgnoreCase);
		var r = await _userManager.AddToRolesAsync(user, others!);
		if (!r.Succeeded)
			_logger.LogError(r.ToLogMessage());

		return r.Succeeded;
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