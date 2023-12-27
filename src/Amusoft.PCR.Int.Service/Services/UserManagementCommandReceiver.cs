using Amusoft.PCR.AM.Service.Interfaces;
using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.Domain.Service.Entities;
using Amusoft.PCR.Domain.Shared.Entities;
using Amusoft.PCR.Domain.Shared.Interfaces;
using Amusoft.PCR.Domain.Shared.ValueTypes;
using Amusoft.PCR.Int.IPC;
using Amusoft.PCR.Int.IPC.Extensions;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using UserPermission = Amusoft.PCR.Int.IPC.UserPermission;
using UserRole = Amusoft.PCR.Int.IPC.UserRole;

namespace Amusoft.PCR.Int.Service.Services;

[Authorize(Policy = PolicyNames.ApiPolicy)]
public class UserManagementCommandReceiver : UserManagement.UserManagementBase
{
	private readonly ILogger<UserManagementCommandReceiver> _logger;
	private readonly IUserManagementRepository _userManagementRepository;
	private readonly IDesktopClientMethods _impersonatedChannel;

	public UserManagementCommandReceiver(ILogger<UserManagementCommandReceiver> logger, IUserManagementRepository userManagementRepository, IDesktopClientMethods impersonatedChannel)
	{
		_logger = logger;
		_userManagementRepository = userManagementRepository;
		_impersonatedChannel = impersonatedChannel;
	}

	public override async Task<ToggleAdministratorResponse> ToggleAdministrator(ToggleAdministratorRequest request, ServerCallContext context)
	{
		var email = GetEmailOrThrow(context);

		var userType = await _userManagementRepository.GetUserTypeAsync(email, context.CancellationToken);
		var newUserType = userType switch
		{
			UserType.Administrator => UserType.User
			, UserType.User => UserType.Administrator
			,
		};

		_logger.LogInformation("{EMail} requesting user type transition to {State}", email, newUserType);

		var r = await _impersonatedChannel.GetConfirmResult(Translations.Generic_Question, string.Format(Translations.Server_RequestOfAdministratorPermissions_0_1, email, newUserType));
		_logger.LogDebug("Prompt result is {Result}", email);
		if (r == true)
		{
			_logger.LogInformation("Attempting to grant Admin user type to {EMail}", email);
			if (await _userManagementRepository.SetUserTypeAsync(email, newUserType, context.CancellationToken))
			{
				_logger.LogInformation("Succeeded to set {EMail} user type {Type}", email, newUserType);
				return new ToggleAdministratorResponse() { Success = true, HasAdmin = newUserType == UserType.Administrator };
			}

			_logger.LogInformation("Failed to change user type for {EMail}", email);
		}

		return new ToggleAdministratorResponse() { Success = false, };
	}

	public override async Task<GetPermissionsReply> GetPermissions(GetPermissionsRequest request, ServerCallContext context)
	{
		var permissionsDto = await _userManagementRepository.GetPermissionsAsync(request.Email, context.CancellationToken);
		if (permissionsDto is null)
			return new GetPermissionsReply()
			{
				Success = false
				, UserId = string.Empty
				, UserPermissions = { ArraySegment<UserPermission>.Empty }
				, UserRoles = { ArraySegment<UserRole>.Empty }
			};
		
		var reply = new GetPermissionsReply();
		reply.UserRoles.Add(permissionsDto.Roles.ToGrpcItems());
		reply.UserPermissions.Add(permissionsDto.Permissions.ToGrpcItems());
		reply.UserId = permissionsDto.UserId;
		reply.UserType = (int)permissionsDto.UserType;
		return reply;
	}

	[Authorize(Roles = RoleNames.Administrator)]
	public override async Task<DefaultResponse> UpdatePermissions(UpdatePermissionsRequest request, ServerCallContext context)
	{
		var email = GetEmailOrThrow(context);

		var userPermissionSet = new UserPermissionSet()
		{
			UserId = request.Email
			, Permissions = request.UserPermissions.ToDomainItems().ToArray()
			, Roles = request.UserRoles.ToDomainItems().ToArray()
		};
		var update = await _userManagementRepository.UpdatePermissionsAsync(email, userPermissionSet, context.CancellationToken);
		return new DefaultResponse() { Success = update, };
	}

	public override async Task<GetRegisteredUsersResponse> GetRegisteredUsers(DefaultRequest request, ServerCallContext context)
	{
		try
		{
			var users = await _userManagementRepository.GetUsersAsync(context.CancellationToken);
			return new GetRegisteredUsersResponse
			{
				Success = true,
				Items = { users.ToGrpcItems() }
			};
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error calling {Method}", nameof(GetRegisteredUsers));
			return new GetRegisteredUsersResponse
			{
				Success = true,
				Items =
				{
					ArraySegment<GetRegisteredUsersResponseItem>.Empty
				}
			};
		}
	}

	public override async Task<DefaultResponse> TryDeleteUser(TryDeleteUserRequest request, ServerCallContext context)
	{
		var actor = GetEmailOrThrow(context);
		if (await _impersonatedChannel.GetConfirmResult(Translations.Generic_Question, string.Format(Translations.Server_UserWantsToDeleteAnotherUser, actor, request.Email)) != true)
		{
			return new DefaultResponse() { Success = false };
		}

		return new DefaultResponse() { Success = await _userManagementRepository.DeleteUserAsync(request.Email) };
	}

	private static string GetEmailOrThrow(ServerCallContext context)
	{
		var email = context.GetHttpContext().User.Identity?.Name;
		if (!string.IsNullOrEmpty(email))
			return email;

		throw new Exception("User identity is empty");
	}
}