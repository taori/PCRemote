using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Amusoft.PCR.Int.Agent.Windows.Events;
using Amusoft.PCR.Int.Agent.Windows.Helpers;
using Amusoft.PCR.Int.Agent.Windows.Interop;
using Amusoft.PCR.Int.Agent.Windows.Windows;
using Amusoft.PCR.Int.IPC;
using Grpc.Core;
using NLog;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Amusoft.PCR.Int.Agent.Windows.Services;

public class DesktopIntegrationServiceImplementation : DesktopIntegrationService.DesktopIntegrationServiceBase, IDisposable
{
	private static readonly Logger Log = LogManager.GetLogger(nameof(DesktopIntegrationServiceImplementation));

	private readonly NativeMonitorManager _monitorManager;

	public DesktopIntegrationServiceImplementation()
	{
		_monitorManager = new NativeMonitorManager();
	}

	public override Task<GetMonitorBrightnessResponse> GetMonitorBrightness(GetMonitorBrightnessRequest request, ServerCallContext context)
	{
		var response = new GetMonitorBrightnessResponse();
		foreach (var managerMonitor in _monitorManager.Monitors)
		{
			response.Items.Add(new GetMonitorBrightnessResponseItem()
			{
				Id = managerMonitor.PhysicalMonitorHandle.ToString(),
				Name = managerMonitor.Description,
				Current = (int)managerMonitor.CurrentValue,
				Min = (int)managerMonitor.MinValue,
				Max = (int)managerMonitor.MaxValue,
			});
		}
		return Task.FromResult(response);
	}

	public override Task<DefaultResponse> SetMonitorBrightness(SetMonitorBrightnessRequest request, ServerCallContext context)
	{
		var match = _monitorManager.Monitors.FirstOrDefault(d => d.PhysicalMonitorHandle.ToString().Equals(request.Id));
		if (match == null)
		{
			Log.Warn("Failed to find a monitor matching Id {Id}", request.Id);
			return Task.FromResult(new DefaultResponse() {Success = false});
		}
		else
		{
			_monitorManager.SetBrightness(match, (uint)request.Value);
			return Task.FromResult(new DefaultResponse() { Success = true });
		}
	}

	public override Task<SuicideOnProcessExitResponse> SuicideOnProcessExit(SuicideOnProcessExitRequest request, ServerCallContext context)
	{
		Log.Info("Executing [{Name}]", nameof(SuicideOnProcessExit));
		var listening = ProcessExitListenerManager.TryObserveProcessExit(request.ProcessId);
		return Task.FromResult(new SuicideOnProcessExitResponse() { Success = listening });
	}

	public override Task<HibernateReply> Hibernate(HibernateRequest request, ServerCallContext context)
	{
		Log.Info("Executing [{Name}]", nameof(Hibernate));
		var success = MachineStateHelper.TryHibernate();
		return Task.FromResult(new HibernateReply() { Success = success });
	}

	public override Task<AbortShutdownReply> AbortShutDown(AbortShutdownRequest request, ServerCallContext context)
	{
		Log.Info("Executing [{Name}]", nameof(AbortShutDown));
		var success = MachineStateHelper.TryAbortShutDown();
		return Task.FromResult(new AbortShutdownReply() { Success = success });
	}

	public override Task<ShutdownDelayedReply> ShutDownDelayed(ShutdownDelayedRequest request, ServerCallContext context)
	{
		Log.Info("Executing [{Name}] [{Delay}s] [{Force}]", nameof(ShutDownDelayed), request.Seconds, request.Force);
		var success = MachineStateHelper.TryShutDownDelayed(TimeSpan.FromSeconds(request.Seconds), request.Force);
		return Task.FromResult(new ShutdownDelayedReply() { Success = success });
	}

	public override Task<RestartReply> Restart(RestartRequest request, ServerCallContext context)
	{
		Log.Info("Executing [{Name}] [{Delay}s] [{Force}]", nameof(Restart), request.Delay, request.Force);
		var success = MachineStateHelper.TryRestart(TimeSpan.FromSeconds(request.Delay), request.Force);
		return Task.FromResult(new RestartReply() { Success = success });
	}

	public override Task<SendKeysReply> SendKeys(SendKeysRequest request, ServerCallContext context)
	{
		Log.Info("Executing [{Name}] {Message}", nameof(SendKeys), request.Message);
		NativeMethods.SendKeys(request.Message);
		return Task.FromResult(new SendKeysReply());
	}

	public override Task<MonitorOnReply> MonitorOn(MonitorOnRequest request, ServerCallContext context)
	{
		Log.Info("Executing [{Name}]", nameof(MonitorOn));
		NativeMethods.Monitor.On();
		return Task.FromResult(new MonitorOnReply());
	}

	public override Task<MonitorOffReply> MonitorOff(MonitorOffRequest request, ServerCallContext context)
	{
		Log.Info("Executing [{Name}]", nameof(MonitorOff));
		NativeMethods.Monitor.Off();
		return Task.FromResult(new MonitorOffReply());
	}

	public override Task<LockWorkStationReply> LockWorkStation(LockWorkStationRequest request, ServerCallContext context)
	{
		Log.Info("Executing [{Name}]", nameof(LockWorkStation));
		NativeMethods.LockWorkStation();
		return Task.FromResult(new LockWorkStationReply());
	}

	public override Task<ProcessListResponse> GetProcessList(ProcessListRequest request, ServerCallContext context)
	{
		Log.Info("Executing [{Name}]", nameof(GetProcessList));
		var result = ProcessHelper.TryGetProcessList(out var processList);
		var response = new ProcessListResponse();
		if (result)
			response.Results.AddRange(processList);

		return Task.FromResult(response);
	}

	public override Task<FocusWindowResponse> FocusWindow(FocusWindowRequest request, ServerCallContext context)
	{
		Log.Info("Executing [{Name}] [{ProcessId}]", nameof(FocusWindow), request.ProcessId);
		var result = NativeMethods.SetForegroundWindow(request.ProcessId);
		return Task.FromResult(new FocusWindowResponse() { Success = result });
	}

	public override Task<KillProcessResponse> KillProcessById(KillProcessRequest request, ServerCallContext context)
	{
		Log.Info("Executing [{Name}] [{ProcessId}]", nameof(KillProcessById), request.ProcessId);
		var result = ProcessHelper.TryKillProcess(request.ProcessId);
		return Task.FromResult(new KillProcessResponse() { Success = result });
	}

	public override Task<LaunchProgramResponse> LaunchProgram(LaunchProgramRequest request, ServerCallContext context)
	{
		Log.Info("Executing [{Name}] [{Program}] [{Arguments}]", nameof(LaunchProgram), request.ProgramName, request.Arguments);
		// integration exe is already executed in user context, therefore no further impersonation is required.
		var result = ProcessHelper.TryLaunchProgram(request.ProgramName, request.Arguments);

		return Task.FromResult(new LaunchProgramResponse() { Success = result });
	}

	public override Task<SendMediaKeysReply> SendMediaKeys(SendMediaKeysRequest request, ServerCallContext context)
	{
		Log.Info("Executing [{Name}] [{MediaKey}]", nameof(SendMediaKeys), request.KeyCode);

		switch (request.KeyCode)
		{
			case SendMediaKeysRequest.Types.MediaKeyCode.NextTrack:
				NativeMethods.PressMediaKey(NativeMethods.MediaKeyCode.NextTrack);
				break;
			case SendMediaKeysRequest.Types.MediaKeyCode.PreviousTrack:
				NativeMethods.PressMediaKey(NativeMethods.MediaKeyCode.PreviousTrack);
				break;
			case SendMediaKeysRequest.Types.MediaKeyCode.PlayPause:
				NativeMethods.PressMediaKey(NativeMethods.MediaKeyCode.PlayPause);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		return Task.FromResult(new SendMediaKeysReply());
	}

	private static async Task<bool> ConfirmAsync(string title, string description)
	{
		var request = new GetConfirmRequest()
		{
			Description = description,
			Title = title,
		};

		var response = await ViewModelSpawner.GetWindowResponseAsync<ConfirmWindow, ConfirmWindowViewModel, GetConfirmRequest, GetConfirmResponse>(request);
		return response.Success;
	}

	public override async Task<GetClipboardResponse> GetClipboard(GetClipboardRequest request, ServerCallContext context)
	{
		Log.Info("Executing [{Name}]", nameof(GetClipboard));
		
		if (await ConfirmAsync("PC Remote 3", $"Send clipboard content to {request.Requestee}?"))
		{
			try
			{
				var content = await ClipboardHelper.GetClipboardAsync(System.Windows.Forms.TextDataFormat.UnicodeText);

				Log.Debug("Returning {Length} characters to client", content.Length);
				return new GetClipboardResponse() { Content = content, Success = content.Length > 0};
			}
			catch (Exception e)
			{
				Log.Error(e, "Failed to read clipboard.");
				return new GetClipboardResponse() { Content = string.Empty, Success = false};
			}
		}
		else
		{
			Log.Warn("Permission denied");
			return new GetClipboardResponse() { Content = string.Empty, Success = false };
		}
	}

	public override async Task<SetClipboardResponse> SetClipboard(SetClipboardRequest request, ServerCallContext context)
	{
		Log.Info("Executing [{Name}]", nameof(GetClipboard));
		if (await ConfirmAsync("PC Remote 3", $"Allow {request.Requestee} to set clipboard?")) 
		{
			try
			{
				Log.Debug("Setting {Length} characters to client", request.Content.Length);
				await ClipboardHelper.SetClipboardAsync(request.Content, System.Windows.Forms.TextDataFormat.UnicodeText);

				return new SetClipboardResponse() { Success = true };
			}
			catch (Exception e)
			{
				Log.Error(e, "Failed to set clipboard.");
				return new SetClipboardResponse() { Success = false };
			}
		}
		else
		{
			Log.Warn("Permission denied");
			throw new RpcException(new Status(StatusCode.PermissionDenied, "Host declined permission"));
		}
	}

	public override Task<SendMouseMoveResponse> SendMouseMove(SendMouseMoveRequest request, ServerCallContext context)
	{
		try
		{
			NativeMethods.Mouse.Move(request.X, request.Y);
			return Task.FromResult(new SendMouseMoveResponse() { Success = true });
		}
		catch (Exception e)
		{
			Log.Error(e, "An error occured while sending mouse movement");
			return Task.FromResult(new SendMouseMoveResponse() { Success = false });
		}
	}

	public override Task<DefaultResponse> SendLeftMouseButtonClick(DefaultRequest request, ServerCallContext context)
	{
		try
		{
			NativeMethods.Mouse.ClickLeft();
			return Task.FromResult(new DefaultResponse() { Success = true });
		}
		catch (Exception e)
		{
			Log.Error(e, "An error occured while sending mouse leftclick");
			return Task.FromResult(new DefaultResponse() { Success = false });
		}
	}

	public override Task<DefaultResponse> SendRightMouseButtonClick(DefaultRequest request, ServerCallContext context)
	{
		try
		{
			NativeMethods.Mouse.ClickRight();
			return Task.FromResult(new DefaultResponse() { Success = true });
		}
		catch (Exception e)
		{
			Log.Error(e, "An error occured while sending mouse rightclick");
			return Task.FromResult(new DefaultResponse() { Success = false });
		}
	}

	public override Task<AudioFeedResponse> GetAudioFeeds(AudioFeedRequest request, ServerCallContext context)
	{
		var response = new AudioFeedResponse();
		var audioFeedQuery = SimpleAudioManager.TryGetAudioFeeds(out var feeds);
		if (audioFeedQuery)
			response.Items.AddRange(feeds);

		response.Success = audioFeedQuery;

		return Task.FromResult(response);
	}

	public override Task<DefaultResponse> UpdateAudioFeed(UpdateAudioFeedRequest request, ServerCallContext context)
	{
		var result = SimpleAudioManager.TryUpdateFeed(request.Item);

		return Task.FromResult(new DefaultResponse() { Success = result });
	}

	public override async Task<StringResponse> SetUserPassword(ChangeUserPasswordRequest request, ServerCallContext context)
	{
		var promptRequest = new GetPromptTextRequest("Prompt", $"Please select a password for user {request.UserName}", "Password");
		var response = await ViewModelSpawner.GetWindowResponseAsync<PromptWindow, PromptWindowModel, GetPromptTextRequest, PromptCompleted>(promptRequest);

		return new StringResponse()
		{
			Content = response.Content,
			Success = !response.Cancelled
		};
	}

	public void Dispose()
	{
		_monitorManager.Dispose();
	}
}