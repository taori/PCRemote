using System.Collections.Immutable;
using Amusoft.PCR.AM.Agent.Interfaces;
using Amusoft.PCR.Domain.Shared.Entities;
using Amusoft.PCR.Domain.Shared.ValueTypes;
using Amusoft.PCR.Int.IPC;
using Amusoft.PCR.Int.IPC.Extensions;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.Agent.Dependencies;

public class DesktopIpcAgentLayer : DesktopIntegrationService.DesktopIntegrationServiceBase
{
	private readonly IDesktopIntegrationProcessor _processor;
	private readonly ILogger<DesktopIpcAgentLayer> _logger;
	private readonly IAgentUserInterface _agentUserInterface;

	public DesktopIpcAgentLayer(IDesktopIntegrationProcessor processor, ILogger<DesktopIpcAgentLayer> logger, IAgentUserInterface agentUserInterface)
	{
		_processor = processor;
		_logger = logger;
		_agentUserInterface = agentUserInterface;
	}
	
	public override Task<DefaultResponse> Ping(DefaultRequest request, ServerCallContext context)
	{
		return Task.FromResult(new DefaultResponse() { Success = true });
	}

	public override Task<DefaultResponse> SetMonitorBrightness(SetMonitorBrightnessRequest request, ServerCallContext context)
	{
		try
		{
			return Task.FromResult(new DefaultResponse() { Success = _processor.TrySetMonitorBrightness(request.Id, request.Value) });
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(SetMonitorBrightness));
			return Task.FromResult(new DefaultResponse() { Success = false });
		}
	}

	public override Task<GetMonitorBrightnessResponse> GetMonitorBrightness(GetMonitorBrightnessRequest request, ServerCallContext context)
	{
		try
		{
			if (!_processor.TryGetMonitors(out var monitors))
				return Task.FromResult(new GetMonitorBrightnessResponse());
			
			var response = new GetMonitorBrightnessResponse();
			foreach (var monitor in monitors)
			{
				response.Items.Add(new GetMonitorBrightnessResponseItem()
				{
					Id = monitor.Id,
					Name = monitor.Description,
					Current = (int)monitor.Current,
					Min = (int)monitor.Min,
					Max = (int)monitor.Max,
				});
			}
			return Task.FromResult(response);
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(GetMonitorBrightness));
			return Task.FromResult(new GetMonitorBrightnessResponse() {Items = { ArraySegment<GetMonitorBrightnessResponseItem>.Empty }});
		}
	}

	public override Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
	{
		throw new NotImplementedException();
	}

	public override Task<CheckIsAuthenticatedResponse> CheckIsAuthenticated(CheckIsAuthenticatedRequest request, ServerCallContext context)
	{
		throw new NotImplementedException();
	}

	public override Task<AuthenticateResponse> Authenticate(AuthenticateRequest request, ServerCallContext context)
	{
		throw new NotImplementedException();
	}

	public override Task<SuicideOnProcessExitResponse> SuicideOnProcessExit(SuicideOnProcessExitRequest request, ServerCallContext context)
	{
		try
		{
			return Task.FromResult(new SuicideOnProcessExitResponse() { Success = _processor.TryListenForProcessExit(request.ProcessId) });
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(SuicideOnProcessExit));
			return Task.FromResult(new SuicideOnProcessExitResponse() { Success = false });
		}
	}

	public override Task<MonitorOnReply> MonitorOn(MonitorOnRequest request, ServerCallContext context)
	{
		try
		{
			return Task.FromResult(new MonitorOnReply() { Success = _processor.TryMonitorsOn() });
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(MonitorOn));
			return Task.FromResult(new MonitorOnReply() { Success = false });
		}
	}

	public override Task<MonitorOffReply> MonitorOff(MonitorOffRequest request, ServerCallContext context)
	{
		try
		{
			return Task.FromResult(new MonitorOffReply() { Success = _processor.TryMonitorsOff() });
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(MonitorOff));
			return Task.FromResult(new MonitorOffReply() { Success = false });
		}
	}

	public override Task<AbortShutdownReply> AbortShutDown(AbortShutdownRequest request, ServerCallContext context)
	{
		try
		{
			return Task.FromResult(new AbortShutdownReply() { Success = _processor.TryAbortShutdown() });
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(AbortShutDown));
			return Task.FromResult(new AbortShutdownReply() { Success = false });
		}
	}

	public override Task<ShutdownDelayedReply> ShutDownDelayed(ShutdownDelayedRequest request, ServerCallContext context)
	{
		try
		{
			return Task.FromResult(new ShutdownDelayedReply() { Success = _processor.TryShutdown(request.Seconds, request.Force) });
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(ShutDownDelayed));
			return Task.FromResult(new ShutdownDelayedReply() { Success = false });
		}
	}

	public override Task<RestartReply> Restart(RestartRequest request, ServerCallContext context)
	{
		try
		{
			return Task.FromResult(new RestartReply() { Success = _processor.TryRestart(request.Delay, request.Force) });
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(Restart));
			return Task.FromResult(new RestartReply() { Success = false });
		}
	}

	public override Task<HibernateReply> Hibernate(HibernateRequest request, ServerCallContext context)
	{
		try
		{
			return Task.FromResult(new HibernateReply() { Success = _processor.TryHibernate() });
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(Hibernate));
			return Task.FromResult(new HibernateReply() { Success = false });
		}
	}

	public override Task<SendKeysReply> SendKeys(SendKeysRequest request, ServerCallContext context)
	{
		try
		{
			return Task.FromResult(new SendKeysReply() { Success = _processor.TrySendKeys(request.Message) });
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(SendKeys));
			return Task.FromResult(new SendKeysReply() { Success = false });
		}
	}

	public override Task<SendMediaKeysReply> SendMediaKeys(SendMediaKeysRequest request, ServerCallContext context)
	{
		try
		{
			return Task.FromResult(new SendMediaKeysReply() { Success = _processor.TrySendMediaKeys((MediaKeyCode)(int)request.KeyCode) });
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(SendMediaKeys));
			return Task.FromResult(new SendMediaKeysReply() { Success = false });
		}
	}

	public override Task<LockWorkStationReply> LockWorkStation(LockWorkStationRequest request, ServerCallContext context)
	{
		try
		{
			return Task.FromResult(new LockWorkStationReply() { Success = _processor.TryLockWorkstation() });
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(LockWorkStation));
			return Task.FromResult(new LockWorkStationReply() { Success = false });
		}
	}

	public override Task<ProcessListResponse> GetProcessList(ProcessListRequest request, ServerCallContext context)
	{
		try
		{
			return Task.FromResult(new ProcessListResponse()
			{
				Results =
				{
					_processor.TryGetProcessList(out var list)
						? list.Select(d => new ProcessListResponseItem()
						{
							ProcessId = d.ProcessId,
							ProcessName = d.ProcessName,
							CpuUsage = d.CpuUsage,
							MainWindowTitle = d.MainWindowTitle,
						}).ToRepeatedField()
						: ImmutableList<ProcessListResponseItem>.Empty
				}
			});
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(GetProcessList));
			return Task.FromResult(new ProcessListResponse() { Results = { ImmutableList<ProcessListResponseItem>.Empty }});
		}
	}

	public override Task<KillProcessResponse> KillProcessById(KillProcessRequest request, ServerCallContext context)
	{
		try
		{
			return Task.FromResult(new KillProcessResponse() { Success = _processor.TryKillProcessById(request.ProcessId) });
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(KillProcessById));
			return Task.FromResult(new KillProcessResponse() { Success = false });
		}
	}

	public override Task<FocusWindowResponse> FocusWindow(FocusWindowRequest request, ServerCallContext context)
	{
		throw new NotImplementedException();
	}

	public override Task<LaunchProgramResponse> LaunchProgram(LaunchProgramRequest request, ServerCallContext context)
	{
		throw new NotImplementedException();
	}

	public override Task<GetHostCommandResponse> GetHostCommands(GetHostCommandRequest request, ServerCallContext context)
	{
		throw new NotImplementedException();
	}

	public override Task<InvokeHostCommandResponse> InvokeHostCommand(InvokeHostCommandRequest request, ServerCallContext context)
	{
		throw new NotImplementedException();
	}

	public override async Task<GetClipboardResponse> GetClipboard(GetClipboardRequest request, ServerCallContext context)
	{
		try
		{
			if (!await _agentUserInterface.ConfirmAsync("PC Remote 3", $"Send clipboard content to {request.Requestee}?"))
			{
				return new GetClipboardResponse() { Success = false, Content = string.Empty };
			}
				
			var r = await _processor.TryGetClipboardAsync();
			return new GetClipboardResponse() { Success = r.Success, Content = r.Value};
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(GetClipboard));
			return new GetClipboardResponse() { Success = false, Content = string.Empty };
		}
	}

	public override async Task<SetClipboardResponse> SetClipboard(SetClipboardRequest request, ServerCallContext context)
	{
		try
		{
			if (!await _agentUserInterface.ConfirmAsync("PC Remote 3", $"Allow {request.Requestee} to set clipboard?"))
			{
				return new SetClipboardResponse() { Success = false };
			}
			
			_logger.LogDebug("Setting {Length} characters to client", request.Content.Length);
			return new SetClipboardResponse() { Success = await _processor.TrySetClipboardAsync(request.Content) };
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(SetClipboard));
			return new SetClipboardResponse() { Success = false };
		}
	}

	public override Task<GetHostNameResponse> GetHostName(GetHostNameRequest request, ServerCallContext context)
	{
		throw new NotImplementedException();
	}

	public override Task<GetNetworkMacAddressesResponse> GetNetworkMacAddresses(GetNetworkMacAddressesRequest request, ServerCallContext context)
	{
		throw new NotImplementedException();
	}

	public override Task<SendMouseMoveResponse> SendMouseMove(SendMouseMoveRequest request, ServerCallContext context)
	{
		try
		{
			return Task.FromResult(new SendMouseMoveResponse() { Success = _processor.TrySendMouseMove(request.X, request.Y) });
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(SendMouseMove));
			return Task.FromResult(new SendMouseMoveResponse() { Success = false });
		}
	}

	public override Task<DefaultResponse> SendLeftMouseButtonClick(DefaultRequest request, ServerCallContext context)
	{
		try
		{
			return Task.FromResult(new DefaultResponse() { Success = _processor.TrySendMouseLeftButtonClick() });
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(SendLeftMouseButtonClick));
			return Task.FromResult(new DefaultResponse() { Success = false });
		}
	}

	public override Task<DefaultResponse> SendRightMouseButtonClick(DefaultRequest request, ServerCallContext context)
	{
		try
		{
			return Task.FromResult(new DefaultResponse() { Success = _processor.TrySendMouseRightButtonClick() });
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(SendRightMouseButtonClick));
			return Task.FromResult(new DefaultResponse() { Success = false });
		}
	}

	public override Task<AudioFeedResponse> GetAudioFeeds(AudioFeedRequest request, ServerCallContext context)
	{
		try
		{
			var r = _processor.TryGetAudioFeeds(out var feeds);
			var conversion = new Func<AudioFeedData, AudioFeedResponseItem>(data => new AudioFeedResponseItem()
			{
				Id = data.Id,
				Name = data.Name,
				Muted = data.Muted,
				Volume = data.Volume,
			});
			return Task.FromResult(new AudioFeedResponse() { Success = r, Items = { r ? feeds.Select(conversion).ToRepeatedField() : ArraySegment<AudioFeedResponseItem>.Empty }});
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(GetAudioFeeds));
			return Task.FromResult(new AudioFeedResponse() { Success = false });
		}
	}

	public override Task<DefaultResponse> UpdateAudioFeed(UpdateAudioFeedRequest request, ServerCallContext context)
	{
		try
		{
			var c = new AudioFeedData(request.Item.Id, request.Item.Name, request.Item.Volume, request.Item.Muted);
			return Task.FromResult(new DefaultResponse() { Success = _processor.TryUpdateAudioFeed(c) });
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(UpdateAudioFeed));
			return Task.FromResult(new DefaultResponse() { Success = false });
		}
	}

	public override async Task<StringResponse> SetUserPassword(ChangeUserPasswordRequest request, ServerCallContext context)
	{
		try
		{
			var password = await _agentUserInterface.PromptPasswordAsync("PC Remote 3", $"Please select a new password for {request.UserName}", "Password");
			return new StringResponse() { Success = password.Success, Content = password.Value};
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(SetUserPassword));
			return new StringResponse() { Success = false, Content = string.Empty};
		}
	}
}