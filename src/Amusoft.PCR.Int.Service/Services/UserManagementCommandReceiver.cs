using Amusoft.PCR.AM.Service.Interfaces;
using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.Domain.Service.Entities;
using Amusoft.PCR.Domain.Shared.Interfaces;
using Amusoft.PCR.Int.IPC;
using Amusoft.PCR.Int.IPC.Extensions;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

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

	public override async Task<DefaultResponse> RequestAdministrator(DefaultRequest request, ServerCallContext context)
	{
		var email = GetEmailOrThrow(context);

		_logger.LogInformation("{EMail} is asking to be granted the administrator user type", email);

		var r = await _impersonatedChannel.GetConfirmResult(Translations.Generic_Question, string.Format(Translations.Server_RequestOfAdministratorPermissions_0, email));
		_logger.LogDebug("Prompt result is {Result}", email);
		if (r == true)
		{
			_logger.LogInformation("Attempting to grant Admin user type to {EMail}", email);
			if (await _userManagementRepository.SetUserTypeAdminAsync(email, context.CancellationToken))
			{
				_logger.LogInformation("Succeeded to grant {EMail} admin user type", email);
				return new DefaultResponse() { Success = true };
			}

			_logger.LogInformation("Failed to grant {EMail} admin user type", email);
		}

		return new DefaultResponse() { Success = false, };
	}

	public override async Task<GetPermissionsReply> GetPermissions(GetPermissionsRequest request, ServerCallContext context)
	{
		var permissionsDto = await _userManagementRepository.GetPermissionsAsync(request.Email, context.CancellationToken);
		var reply = new GetPermissionsReply();
		reply.Items.Add(permissionsDto.ToGrpcItems());
		return reply;
	}

	[Authorize(Roles = RoleNames.Administrator)]
	public override async Task<DefaultResponse> UpdatePermissions(UpdatePermissionsRequest request, ServerCallContext context)
	{
		var email = GetEmailOrThrow(context);

		var update = await _userManagementRepository.UpdatePermissionsAsync(email, request.Items.ToDomainItems(), context.CancellationToken);
		return new DefaultResponse() { Success = update, };
	}

	private static string GetEmailOrThrow(ServerCallContext context)
	{
		var email = context.GetHttpContext().User.Identity?.Name;
		if (!string.IsNullOrEmpty(email))
			return email;

		throw new Exception("User identity is empty");
	}
}