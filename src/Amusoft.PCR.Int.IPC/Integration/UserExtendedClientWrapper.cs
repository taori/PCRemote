using Amusoft.PCR.Domain.Shared.Entities;
using Amusoft.PCR.Domain.Shared.Interfaces;
using Amusoft.PCR.Domain.Shared.ValueTypes;
using Amusoft.PCR.Int.IPC.Extensions;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.IPC.Integration;

public class UserExtendedClientWrapper : IIdentityExtendedClient
{
	private readonly UserManagement.UserManagementClient _client;
	private readonly ILogger<UserExtendedClientWrapper> _logger;

	public UserExtendedClientWrapper(UserManagement.UserManagementClient client, ILogger<UserExtendedClientWrapper> logger)
	{
		_client = client;
		_logger = logger;
	}

	public async Task<bool?> RequestAdminToggle(string email, CancellationToken cancellationToken)
	{
		try
		{
			var reply = await _client.ToggleAdministratorAsync(new ToggleAdministratorRequest() { Email = email }, cancellationToken: cancellationToken);
			return reply.HasAdmin && reply.Success;
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(RequestAdminToggle));
			return default;
		}
	}

	public async Task<UserPermissionSet?> GetPermissionsAsync(string email, CancellationToken cancellationToken)
	{
		try
		{
			var reply = await _client.GetPermissionsAsync(new GetPermissionsRequest() { Email = email }, cancellationToken: cancellationToken);
			return new UserPermissionSet()
			{
				UserId = reply.UserId
				, UserType = (UserType)reply.UserType
				, Permissions = reply.UserPermissions.ToDomainItems().ToArray()
				, Roles = reply.UserRoles.ToDomainItems().ToArray()
			};
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(GetPermissionsAsync));
			return default;
		}
	}

	public async Task<bool?> UpdatePermissionsAsync(string email, UserPermissionSet permissions, CancellationToken cancellationToken)
	{
		try
		{
			var request = new UpdatePermissionsRequest()
			{
				Email = email
				, UserPermissions = { permissions.Permissions.ToGrpcItems() }
				, UserRoles = { permissions.Roles.ToGrpcItems() }
			};
			var reply = await _client.UpdatePermissionsAsync(request, cancellationToken: cancellationToken);
			return reply.Success;
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(UpdatePermissionsAsync));
			return default;
		}
	}

	public async Task<RegisteredUser[]?> GetRegisteredUsersAsync(CancellationToken cancellationToken)
	{
		try
		{
			var reply = await _client.GetRegisteredUsersAsync(new DefaultRequest(), cancellationToken: cancellationToken);
			return reply.Success ? reply.Items.ToDomainItems().ToArray() : Array.Empty<RegisteredUser>();
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(GetRegisteredUsersAsync));
			return default;
		}
	}

	public async Task<bool?> TryDeleteUserAsync(string email, CancellationToken cancellationToken)
	{
		try
		{
			var reply = await _client.TryDeleteUserAsync(new TryDeleteUserRequest() { Email = email }, cancellationToken: cancellationToken);
			return reply.Success;
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(TryDeleteUserAsync));
			return default;
		}
	}
}