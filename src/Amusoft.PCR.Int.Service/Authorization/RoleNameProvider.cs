using System.Reflection;
using Amusoft.PCR.AM.Service.Interfaces;
using Amusoft.PCR.Domain.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.Service.Authorization;

public class DefaultRoleNameProvider : IRoleNameProvider
{
	public IEnumerable<string> GetRoleNames()
	{
		yield return RoleNames.Administrator;
		yield return RoleNames.Functions;
		yield return RoleNames.Processes;
		yield return RoleNames.Computer;
		yield return RoleNames.Audio;
		yield return RoleNames.ActiveWindow;
	}
}
public interface IRoleNameProvider
{
	IEnumerable<string> GetRoleNames();
}

public class BackendAuthorizeRoleProvider(
		ILogger<BackendAuthorizeRoleProvider> log, 
		IEnumerable<IMethodBasedRoleProvider> methodBasedRoleProviders)
	: IRoleNameProvider
{
	public IEnumerable<string> GetRoleNames()
	{
		var allMethods = methodBasedRoleProviders.SelectMany(d => d.GetMethods()).ToArray();
		var methods = allMethods;
		var roleNames = new HashSet<string>();

		foreach (var method in methods)
		{
			var authorizeAttribute = method.GetCustomAttribute<AuthorizeAttribute>();
			if (authorizeAttribute == null)
				continue;

			if(string.IsNullOrEmpty(authorizeAttribute.Roles))
				continue;

			var roles = authorizeAttribute.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries);
			foreach (var role in roles)
			{
				log.LogDebug("Adding role {RoleName} as declared on method {MethodName}", role, method.Name);
				roleNames.Add(role);
			}
		}

		return roleNames;
	}
}