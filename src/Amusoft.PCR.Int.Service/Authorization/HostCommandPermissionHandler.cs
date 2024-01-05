using Amusoft.PCR.Domain.Service.Entities;
using Amusoft.PCR.Domain.Shared.Entities;
using Amusoft.PCR.Domain.Shared.ValueTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Amusoft.PCR.Int.Service.Authorization;

public class HostCommandPermissionHandler : AuthorizationHandler<HostCommandPermissionRequirement, HostCommand>
{
	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HostCommandPermissionRequirement requirement,
		HostCommand resource)
	{
		if (context.User.IsInRole(RoleNames.Administrator))
		{
			context.Succeed(requirement);
			return Task.CompletedTask;
		}

		if (context.User.HasClaim(d => d.Type == PermissionClaimNames.ApplicationPermissionClaim
		                               && d.Value == resource.Id 
		                               && d.ValueType == ((int) PermissionKind.HostCommand).ToString()))
		{
			context.Succeed(requirement);
			return Task.CompletedTask;
		}

		return Task.CompletedTask;
	}
}

public class RoleOrAdminAuthorizationHandler : IAuthorizationHandler
{
	public Task HandleAsync(AuthorizationHandlerContext context)
	{
		var rolesAuthorizationRequirement = context
			.Requirements
			.OfType<RolesAuthorizationRequirement>()
			.FirstOrDefault();

		if (rolesAuthorizationRequirement != null)
		{
			if (context.User.IsInRole(RoleNames.Administrator))
			{
				context.Succeed(rolesAuthorizationRequirement);
				return Task.CompletedTask;
			}
		}

		return Task.CompletedTask;
	}
}