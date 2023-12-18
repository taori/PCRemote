using Amusoft.PCR.Domain.Shared.Interfaces;

namespace Amusoft.PCR.Int.IPC.Integration;

public class UserManagementClientWrapper : IIdentityManagementMethods
{
	private readonly UserManagement.UserManagementClient _client;

	public UserManagementClientWrapper(UserManagement.UserManagementClient _client)
	{
		this._client = _client;
	}
}