using Amusoft.PCR.Domain.Shared.Interfaces;
using Amusoft.PCR.Int.IPC;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.UI.ProjectDependencies;

public class PermissionAcquisionService : IPermissionAcquisionService
{
	private readonly UserManagement.UserManagementClient _userManagementClient;
	private readonly ILogger<PermissionAcquisionService> _logger;

	public PermissionAcquisionService(UserManagement.UserManagementClient userManagementClient, ILogger<PermissionAcquisionService> logger)
	{
		_userManagementClient = userManagementClient;
		_logger = logger;
	}

	public async Task<bool> AcquireRolesAsync(string[] roles)
	{
		try
		{
			var r = await _userManagementClient.TryRequestRolesAsync(new TryRequestRolesRequest() { Roles = { roles } });
			return r.Success;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to request roles {Roles}", string.Join(",", roles));
			return false;
		}
	}
}