using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Int.IPC;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace Amusoft.PCR.App.Service.Services;

public class DesktopIntegrationService : Int.IPC.DesktopIntegrationService.DesktopIntegrationServiceBase
{
	private readonly IDesktopClientMethods _impersonatedChannel;

	public DesktopIntegrationService(IDesktopClientMethods impersonatedChannel)
	{
		_impersonatedChannel = impersonatedChannel;
	}
	
	public override async Task<SendMouseMoveResponse> SendMouseMove(IAsyncStreamReader<SendMouseMoveRequestItem> requestStream, ServerCallContext context)
	{
		await _impersonatedChannel.SendMouseMoveAsync(requestStream, context.CancellationToken);
		return new SendMouseMoveResponse();
	}
	
	public override async Task<DefaultResponse> SendLeftMouseButtonClick(DefaultRequest request, ServerCallContext context)
	{
		var result = await _impersonatedChannel.SendLeftMouseClickAsync();
		return new DefaultResponse() { Success = result == true };
	}
	
	public override async Task<DefaultResponse> SendRightMouseButtonClick(DefaultRequest request, ServerCallContext context)
	{
		var result = await _impersonatedChannel.SendRightMouseClickAsync();
		return new DefaultResponse() { Success = result == true };
	}
	
	public override async Task<GetClipboardResponse> GetClipboard(GetClipboardRequest request, ServerCallContext context)
	{
		var result = await _impersonatedChannel.GetClipboardAsync(request.Requestee);
		return new GetClipboardResponse()
		{
			Content = result
		};
	}
	
	public override async Task<SetClipboardResponse> SetClipboard(SetClipboardRequest request, ServerCallContext context)
	{
		var result = await _impersonatedChannel.SetClipboardAsync(request.Requestee, request.Content);
		return new SetClipboardResponse()
		{
			Success = result == true
		};
	}

	// [AllowAnonymous]
	// public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
	// {
	// 	var tokenData = await _jwtTokenService.CreateAuthenticationAsync(request.User, request.Password);
	// 	return new LoginResponse()
	// 	{
	// 		AccessToken = tokenData.AccessToken,
	// 		RefreshToken = tokenData.RefreshToken,
	// 		InvalidCredentials = tokenData.InvalidCredentials
	// 	};
	// }

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

	public override async Task<MonitorOnReply> MonitorOn(MonitorOnRequest request, ServerCallContext context)
	{
		var result = await _impersonatedChannel.MonitorOn();
		return new MonitorOnReply()
		{
			Success = result == true
		};
	}

	public override async Task<MonitorOffReply> MonitorOff(MonitorOffRequest request, ServerCallContext context)
	{
		var result = await _impersonatedChannel.MonitorOff();
		return new MonitorOffReply()
		{
			Success = result == true
		};
	}
	
	public override async Task<AbortShutdownReply> AbortShutDown(AbortShutdownRequest request, ServerCallContext context)
	{
		var success = await _impersonatedChannel.AbortShutdown();
		return new AbortShutdownReply()
		{
			Success = success == true
		};
	}
	
	public override async Task<ShutdownDelayedReply> ShutDownDelayed(ShutdownDelayedRequest request, ServerCallContext context)
	{
		var success = await _impersonatedChannel.Shutdown(TimeSpan.FromSeconds(request.Seconds), request.Force);
		return new ShutdownDelayedReply()
		{
			Success = success == true
		};
	}
	
	public override async Task<RestartReply> Restart(RestartRequest request, ServerCallContext context)
	{
		var success = await _impersonatedChannel.Restart(TimeSpan.FromSeconds(request.Delay), request.Force);
		return new RestartReply()
		{
			Success = success == true
		};
	}
	
	public override async Task<HibernateReply> Hibernate(HibernateRequest request, ServerCallContext context)
	{
		var success = await _impersonatedChannel.Hibernate();
		return new HibernateReply()
		{
			Success = success == true
		};
	}
	
	public override async Task<AudioFeedResponse> GetAudioFeeds(AudioFeedRequest request, ServerCallContext context)
	{
		var value = await _impersonatedChannel.GetAudioFeedsResponse();
		return value ?? new AudioFeedResponse() {Success = value is {Success: true}};
	}

	public override async Task<DefaultResponse> UpdateAudioFeed(UpdateAudioFeedRequest request, ServerCallContext context)
	{
		var value = await _impersonatedChannel.UpdateAudioFeed(request);
		return value ?? new DefaultResponse() { Success = value?.Success ?? false };
	}
	
	public override async Task<SendKeysReply> SendKeys(SendKeysRequest request, ServerCallContext context)
	{
		var success = await _impersonatedChannel.SendKeys(request.Message);
		return new SendKeysReply()
		{
			Success = success == true
		};
	}
	
	public override async Task<SendMediaKeysReply> SendMediaKeys(SendMediaKeysRequest request, ServerCallContext context)
	{
		var success = await _impersonatedChannel.SendMediaKey(request.KeyCode);
		return new SendMediaKeysReply()
		{
			Success = success == true
		};
	}
	
	public override async Task<LockWorkStationReply> LockWorkStation(LockWorkStationRequest request, ServerCallContext context)
	{
		var success = await _impersonatedChannel.LockWorkStation();
		return new LockWorkStationReply()
		{
			Success = success == true
		};
	}
	
	public override async Task<ProcessListResponse> GetProcessList(ProcessListRequest request, ServerCallContext context)
	{
		var results = await _impersonatedChannel.GetProcessList();
		var processListResponse = new ProcessListResponse();
		processListResponse.Results.AddRange(results.Value);
		return processListResponse;
	}
	
	public override async Task<KillProcessResponse> KillProcessById(KillProcessRequest request, ServerCallContext context)
	{
		var success = await _impersonatedChannel.KillProcessById(request.ProcessId);
		return new KillProcessResponse()
		{
			Success = success == true
		};
	}
	
	public override async Task<FocusWindowResponse> FocusWindow(FocusWindowRequest request, ServerCallContext context)
	{
		var success = await _impersonatedChannel.FocusProcessWindow(request.ProcessId);
		return new FocusWindowResponse()
		{
			Success = success == true
		};
	}
	
	public override async Task<LaunchProgramResponse> LaunchProgram(LaunchProgramRequest request, ServerCallContext context)
	{
		if (request.Arguments == null)
		{
			var success = await _impersonatedChannel.LaunchProgram(request.ProgramName);
			return new LaunchProgramResponse()
			{
				Success = success == true
			};
		}
		else
		{
			var success = await _impersonatedChannel.LaunchProgram(request.ProgramName, request.Arguments);
			return new LaunchProgramResponse()
			{
				Success = success == true
			};
		}
	}
	
	// public override async Task<GetHostCommandResponse> GetHostCommands(GetHostCommandRequest request, ServerCallContext context)
	// {
	// 	var commands = await _dbContext.HostCommands.ToListAsync();
	// 	var response = new GetHostCommandResponse();
	//
	// 	foreach (var command in commands)
	// 	{
	// 		var authorizeResult = await _authorizationService.AuthorizeAsync(context.GetHttpContext().User, command, PolicyNames.ApplicationPermissionPolicy);
	// 		_logger.LogDebug("User {User} authorization status for command {Id} is {Status}", context.GetHttpContext().User, command.Id, authorizeResult.Succeeded);
	// 		if (authorizeResult.Succeeded)
	// 		{
	// 			response.Results.Add(new GetHostCommandResponseItem()
	// 			{
	// 				CommandId = command.Id,
	// 				Title = command.CommandName
	// 			});
	// 		}
	// 	}
	//
	// 	return response;
	// }
	
	// public override async Task<InvokeHostCommandResponse> InvokeHostCommand(InvokeHostCommandRequest request, ServerCallContext context)
	// {
	// 	var command = await _dbContext.HostCommands.FindAsync(request.Id);
	// 	if (command == null)
	// 	{
	// 		throw new RpcException(new Status(StatusCode.NotFound, $"Command {request.Id} not found"));
	// 	}
	//
	// 	var success = await _channel.LaunchProgram(command.ProgramPath, command.Arguments);
	//
	// 	return new InvokeHostCommandResponse()
	// 	{
	// 		Success = success
	// 	};
	// }

	[AllowAnonymous]
	public override Task<GetHostNameResponse> GetHostName(GetHostNameRequest request, ServerCallContext context)
	{
		return Task.FromResult(new GetHostNameResponse() { Content = System.Environment.MachineName });
	}
	
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
		var result = await _impersonatedChannel.SetUserPassword(request);
		if (result is {Success: true})
			return new StringResponse() {Content = result.Content, Success = true};

		return new StringResponse() {Success = false, Content = string.Empty};
	}
}