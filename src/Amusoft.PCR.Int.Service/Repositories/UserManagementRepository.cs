using Amusoft.PCR.AM.Service.Interfaces;
using Amusoft.PCR.Domain.Service.Entities;
using Amusoft.PCR.Domain.Service.ValueTypes;
using Amusoft.PCR.Int.Service.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.Service.Repositories;

public class UserManagementRepository : IUserManagementRepository
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly ILogger<UserManagementRepository> _logger;

	public UserManagementRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<UserManagementRepository> logger)
	{
		_userManager = userManager;
		_roleManager = roleManager;
		_logger = logger;
	}

	public async Task<UserPermission[]> GetPermissionsAsync(string email, CancellationToken cancellationToken)
	{
		var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
		if (user is null)
			return Array.Empty<UserPermission>();

		var allRoles = await _roleManager.Roles.ToArrayAsync(cancellationToken).ConfigureAwait(false);
		var userRoles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

		return Array.Empty<UserPermission>();
	}

	public async Task<bool> SetUserTypeAdminAsync(string email, CancellationToken cancellationToken)
	{
		if (await TryFindUserByMailAsync(email) is var user && user is null)
			return false;

		user.UserType = UserType.Administrator;
		var result = await _userManager.UpdateAsync(user);
		if (result is { Succeeded: false })
		{
			_logger.LogError(string.Join("; ", result.Errors.Select(d => $"{d.Code}, {d.Description}")));
		}

		return result.Succeeded;
	}

	public async Task<bool> UpdatePermissionsAsync(string email, IEnumerable<UserPermission> items, CancellationToken cancellationToken)
	{
		if (await TryFindUserByMailAsync(email) is var user && user is null)
			return false;

		var userRoles = new HashSet<string>(await _userManager.GetRolesAsync(user), StringComparer.OrdinalIgnoreCase);
		var updateSet = items.Select(permission => (permission, _userManager.IsInRoleAsync(user, permission.SubjectId)));
		return false;
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