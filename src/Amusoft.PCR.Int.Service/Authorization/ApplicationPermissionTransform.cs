using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.Service.Authorization;


public class PermissionClaimNames
{
	public const string TransformDone = nameof(TransformDone);
	public const string ApplicationPermissionClaim = nameof(ApplicationPermissionClaim);
}

public class ApplicationPermissionTransform : IClaimsTransformation
{
	private readonly ApplicationDbContext _dbContext;
	private readonly ILogger<ApplicationPermissionTransform> _log;
	private readonly UserManager<ApplicationUser> _userManager;

	public ApplicationPermissionTransform(ApplicationDbContext dbContext, ILogger<ApplicationPermissionTransform> log, UserManager<ApplicationUser> userManager)
	{
		_dbContext = dbContext;
		_log = log;
		_userManager = userManager;
	}

	public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
	{
		if (principal.HasClaim(d => d.Type == PermissionClaimNames.TransformDone))
			return principal;

		return await GetAppendedPrincipalAsync(principal);
	}

	private async Task<ClaimsPrincipal> GetAppendedPrincipalAsync(ClaimsPrincipal original)
	{
		var principal = new ClaimsPrincipal(original);
		var identifierClaim = original.Identities
			.Select(d => d.Claims.FirstOrDefault(f => f.Type == ClaimTypes.NameIdentifier))
			.FirstOrDefault();
		if (identifierClaim == null)
			throw new Exception("No name identifier claim present in authenticated user.");

		var permissions = await _dbContext.Permissions
			.Where(d => d.UserId == identifierClaim.Value)
			.ToListAsync();
		var identity = principal.Identities
			.FirstOrDefault();
		if (identity == null)
		{
			_log.LogError("Permission system is malfunctioning. There should not ever be a principal without an identity");
			return original;
		}

		if (await _userManager.FindByIdAsync(identifierClaim.Value) is { } user)
		{
			foreach (var normalizedRole in await _userManager.GetRolesAsync(user))
			{
				if (!identity.HasClaim(ClaimTypes.Role, normalizedRole))
					identity.AddClaim(new Claim(ClaimTypes.Role, normalizedRole));
			}
		}

		_log.LogDebug("Adding transform done flag to claims");
		identity.AddClaim(new Claim(PermissionClaimNames.TransformDone, "1"));
		_log.LogDebug("Adding permissions to identity");
		foreach (var permission in permissions)
		{
			identity.AddClaim(new Claim(PermissionClaimNames.ApplicationPermissionClaim, permission.SubjectId, ((int)permission.PermissionType).ToString()));
		}

		return new ClaimsPrincipal(original.Identities);
	}
}