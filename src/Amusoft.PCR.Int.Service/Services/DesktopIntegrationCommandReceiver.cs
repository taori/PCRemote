using System.Net.NetworkInformation;
using System.Reflection;
using Amusoft.PCR.AM.Service.Interfaces;
using Amusoft.PCR.Domain.Service.Entities;
using Amusoft.PCR.Domain.Shared.Entities;
using Amusoft.PCR.Domain.Shared.Interfaces;
using Amusoft.PCR.Int.IPC;
using Amusoft.PCR.Int.IPC.Extensions;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.Service.Services;

[Authorize(Policy = PolicyNames.ApiPolicy)]
public class DesktopIntegrationCommandReceiver(
	IHostCommandService hostCommandService
	, ILogger<DesktopIntegrationCommandReceiver> logger
	,
	IAuthorizationService authorizationService,
	IDesktopClientMethods impersonatedChannel) 
	: DesktopIntegrationService.DesktopIntegrationServiceBase, IMethodBasedRoleProvider
{
	[Authorize(Roles = RoleNames.FunctionMonitorControl)]
	public override async Task<DefaultResponse> SetMonitorBrightness(SetMonitorBrightnessRequest request, ServerCallContext context)
	{
		var result = await impersonatedChannel.SetMonitorBrightness(request.Id, request.Value);
		return new DefaultResponse() {Success = result == true};
	}

	public override async Task<GetMonitorBrightnessResponse> GetMonitorBrightness(GetMonitorBrightnessRequest request, ServerCallContext context)
	{
		var result = await impersonatedChannel.GetMonitorBrightness();
		if (result.Success)
			return new GetMonitorBrightnessResponse() {Items = {result.Value.ToGrpcItems()}};

		return new GetMonitorBrightnessResponse() { };
	}

	[Authorize(Roles = RoleNames.FunctionMouseControl)]
	public override async Task<SendMouseMoveResponse> SendMouseMove(SendMouseMoveRequest request, ServerCallContext context)
	{
		var success = await impersonatedChannel.SendMouseMoveAsync(request.X, request.Y);
		return new SendMouseMoveResponse()
		{
			Success = success ?? false
		};
	}
	
	[Authorize(Roles = RoleNames.FunctionMouseControl)]
	public override async Task<DefaultResponse> SendLeftMouseButtonClick(DefaultRequest request, ServerCallContext context)
	{
		var result = await impersonatedChannel.SendLeftMouseClickAsync();
		return new DefaultResponse() { Success = result == true };
	}
	
	[Authorize(Roles = RoleNames.FunctionMouseControl)]
	public override async Task<DefaultResponse> SendRightMouseButtonClick(DefaultRequest request, ServerCallContext context)
	{
		var result = await impersonatedChannel.SendRightMouseClickAsync();
		return new DefaultResponse() { Success = result == true };
	}
	
	[Authorize(Roles = RoleNames.FunctionReadClipboard)]
	public override async Task<GetClipboardResponse> GetClipboard(GetClipboardRequest request, ServerCallContext context)
	{
		var result = await impersonatedChannel.GetClipboardAsync(request.Requestee);
		return new GetClipboardResponse()
		{
			Content = result,
			Success = result is {Length: >0}
		};
	}
	
	[Authorize(Roles = RoleNames.FunctionWriteClipboard)]
	public override async Task<SetClipboardResponse> SetClipboard(SetClipboardRequest request, ServerCallContext context)
	{
		var result = await impersonatedChannel.SetClipboardAsync(request.Requestee, request.Content);
		return new SetClipboardResponse()
		{
			Success = result == true
		};
	}

	[Authorize]
	public override async Task<CheckIsAuthenticatedResponse> CheckIsAuthenticated(CheckIsAuthenticatedRequest request, ServerCallContext context)
	{
		var authenticated = await IsContextAuthenticated(context);
		return new CheckIsAuthenticatedResponse() { Result = authenticated };
	}

	private static Task<bool> IsContextAuthenticated(ServerCallContext context)
	{
		return Task.FromResult(context.GetHttpContext()?.User?.Identity?.IsAuthenticated ?? false);
	}

	[Authorize]
	public override Task<AuthenticateResponse> Authenticate(AuthenticateRequest request, ServerCallContext context)
	{
		return Task.FromResult(new AuthenticateResponse() { Success = true });
	}

	[Authorize(Roles = RoleNames.FunctionMonitorControl)]
	public override async Task<MonitorOnReply> MonitorOn(MonitorOnRequest request, ServerCallContext context)
	{
		var result = await impersonatedChannel.MonitorOn();
		return new MonitorOnReply()
		{
			Success = result == true
		};
	}

	[Authorize(Roles = RoleNames.FunctionMonitorControl)]
	public override async Task<MonitorOffReply> MonitorOff(MonitorOffRequest request, ServerCallContext context)
	{
		var result = await impersonatedChannel.MonitorOff();
		return new MonitorOffReply()
		{
			Success = result == true
		};
	}
	
	[Authorize(Roles = RoleNames.FunctionShutdownCancel)]
	public override async Task<AbortShutdownReply> AbortShutDown(AbortShutdownRequest request, ServerCallContext context)
	{
		var success = await impersonatedChannel.AbortShutdown();
		return new AbortShutdownReply()
		{
			Success = success == true
		};
	}
	
	[Authorize(Roles = RoleNames.FunctionMonitorControl)]
	public override async Task<ShutdownDelayedReply> ShutDownDelayed(ShutdownDelayedRequest request, ServerCallContext context)
	{
		var success = await impersonatedChannel.Shutdown(TimeSpan.FromSeconds(request.Seconds), request.Force);
		return new ShutdownDelayedReply()
		{
			Success = success == true
		};
	}
	
	[Authorize(Roles = RoleNames.FunctionRestart)]
	public override async Task<RestartReply> Restart(RestartRequest request, ServerCallContext context)
	{
		var success = await impersonatedChannel.Restart(TimeSpan.FromSeconds(request.Delay), request.Force);
		return new RestartReply()
		{
			Success = success == true
		};
	}
	
	[Authorize(Roles = RoleNames.FunctionHibernate)]
	public override async Task<HibernateReply> Hibernate(HibernateRequest request, ServerCallContext context)
	{
		var success = await impersonatedChannel.Hibernate();
		return new HibernateReply()
		{
			Success = success == true
		};
	}
	
	public override async Task<AudioFeedResponse> GetAudioFeeds(AudioFeedRequest request, ServerCallContext context)
	{
		var value = await impersonatedChannel.GetAudioFeeds();
		if (value.Success)
		{
			return new AudioFeedResponse()
			{
				Success = true,
				Items = { value.Value.ToGrpcItems() }
			};
		}

		return new AudioFeedResponse()
		{
			Success = false,
			Items = { ArraySegment<AudioFeedResponseItem>.Empty }
		};
	}

	[Authorize(Roles = RoleNames.Audio)]
	public override async Task<DefaultResponse> UpdateAudioFeed(UpdateAudioFeedRequest request, ServerCallContext context)
	{
		var value = await impersonatedChannel.UpdateAudioFeed(request.Item.ToDomainItem());
		return new DefaultResponse() { Success = value == true };
	}
	
	[Authorize(Roles = RoleNames.FunctionSendInput)]
	public override async Task<SendKeysReply> SendKeys(SendKeysRequest request, ServerCallContext context)
	{
		var success = await impersonatedChannel.SendKeys(request.Message);
		return new SendKeysReply()
		{
			Success = success == true
		};
	}
	
	[Authorize(Roles = RoleNames.FunctionSendInput)]
	public override async Task<SendMediaKeysReply> SendMediaKeys(SendMediaKeysRequest request, ServerCallContext context)
	{
		var success = await impersonatedChannel.SendMediaKey(request.KeyCode.ToDomainType());
		return new SendMediaKeysReply()
		{
			Success = success == true
		};
	}
	
	[Authorize(Roles = RoleNames.FunctionLockWorkstation)]
	public override async Task<LockWorkStationReply> LockWorkStation(LockWorkStationRequest request, ServerCallContext context)
	{
		var success = await impersonatedChannel.LockWorkStation();
		return new LockWorkStationReply()
		{
			Success = success == true
		};
	}
	
	public override async Task<ProcessListResponse> GetProcessList(ProcessListRequest request, ServerCallContext context)
	{
		var results = await impersonatedChannel.GetProcessList();
		var processListResponse = new ProcessListResponse();
		processListResponse.Results.AddRange(results.Value.ToGrpcItems());
		return processListResponse;
	}
	
	[Authorize(Roles = RoleNames.FunctionKillProcessById)]
	public override async Task<KillProcessResponse> KillProcessById(KillProcessRequest request, ServerCallContext context)
	{
		var success = await impersonatedChannel.KillProcessById(request.ProcessId);
		return new KillProcessResponse()
		{
			Success = success == true
		};
	}
	
	[Authorize(Roles = RoleNames.FunctionFocusWindow)]
	public override async Task<FocusWindowResponse> FocusWindow(FocusWindowRequest request, ServerCallContext context)
	{
		var success = await impersonatedChannel.FocusProcessWindow(request.ProcessId);
		return new FocusWindowResponse()
		{
			Success = success == true
		};
	}
	
	[Authorize(Roles = RoleNames.FunctionLaunchProgram)]
	public override async Task<LaunchProgramResponse> LaunchProgram(LaunchProgramRequest request, ServerCallContext context)
	{
		if (request.Arguments == null)
		{
			var success = await impersonatedChannel.LaunchProgram(request.ProgramName);
			return new LaunchProgramResponse()
			{
				Success = success == true
			};
		}
		else
		{
			var success = await impersonatedChannel.LaunchProgram(request.ProgramName, request.Arguments);
			return new LaunchProgramResponse()
			{
				Success = success == true
			};
		}
	}
	
	[Authorize(Roles = RoleNames.FunctionLaunchProgram)]
	public override async Task<GetHostCommandResponse> GetHostCommands(GetHostCommandRequest request, ServerCallContext context)
	{
		var commands = await hostCommandService.GetAllAsync();
		var response = new GetHostCommandResponse();
	
		foreach (var command in commands)
		{
			var authorizeResult = await authorizationService.AuthorizeAsync(context.GetHttpContext().User, command, PolicyNames.FunctionPermissionPolicy);
			logger.LogDebug("User {User} authorization status for command {Id} is {Status}", context.GetHttpContext().User, command.Id, authorizeResult.Succeeded);
			if (authorizeResult.Succeeded)
			{
				response.Results.Add(new GetHostCommandResponseItem()
				{
					CommandId = command.Id,
					Title = command.CommandName
				});
			}
		}
	
		return response;
	}
	
	[Authorize(Roles = RoleNames.FunctionLaunchProgram)]
	public override async Task<InvokeHostCommandResponse> InvokeHostCommand(InvokeHostCommandRequest request, ServerCallContext context)
	{
		var command = await hostCommandService.GetByIdAsync(request.Id);
		if (command == null)
		{
			throw new RpcException(new Status(StatusCode.NotFound, $"Command {request.Id} not found"));
		}
	
		var success = await impersonatedChannel.LaunchProgram(command.ProgramPath, command.Arguments);
	
		return new InvokeHostCommandResponse()
		{
			Success = success == true,
		};
	}

	[AllowAnonymous]
	public override Task<GetHostNameResponse> GetHostName(GetHostNameRequest request, ServerCallContext context)
	{
		return Task.FromResult(new GetHostNameResponse() { Content = Environment.MachineName });
	}
	
	[Authorize]
	public override Task<GetNetworkMacAddressesResponse> GetNetworkMacAddresses(GetNetworkMacAddressesRequest request, ServerCallContext context)
	{
		var response = new GetNetworkMacAddressesResponse();
		var interfaces = NetworkInterface.GetAllNetworkInterfaces()
			.Where(d => d.OperationalStatus == OperationalStatus.Up
						&& d.NetworkInterfaceType == NetworkInterfaceType.Ethernet);
		response.Results.AddRange(interfaces.Select(d => d.GetPhysicalAddress().ToString()).Select(d => new GetNetworkMacAddressesResponseItem() { MacAddress = d }));

		return Task.FromResult(response);
	}

	[AllowAnonymous]
	public override async Task<StringResponse> SetUserPassword(ChangeUserPasswordRequest request, ServerCallContext context)
	{
		var result = await impersonatedChannel.SetUserPassword(request.UserName);
		if (result is {Success: true})
			return new StringResponse() {Content = result.Value, Success = true};

		return new StringResponse() {Success = false, Content = string.Empty};
	}

	MethodInfo[] IMethodBasedRoleProvider.GetMethods()
	{
		return typeof(DesktopIntegrationCommandReceiver).GetMethods();
	}
}