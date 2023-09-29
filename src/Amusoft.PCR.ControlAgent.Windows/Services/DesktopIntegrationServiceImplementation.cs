using Amusoft.PCR.ControlAgent.Shared;
using Amusoft.PCR.Grpc.Common;
using Grpc.Core;
using Microsoft.Extensions.Configuration.UserSecrets;
using NLog;
using System.Threading.Tasks;
using System;
using System.Windows.Forms;
using Amusoft.PCR.ControlAgent.Windows.Events;
using Amusoft.PCR.ControlAgent.Windows.Helpers;
using Amusoft.PCR.ControlAgent.Windows.Interop;
using Amusoft.PCR.ControlAgent.Windows.Windows;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Amusoft.PCR.ControlAgent.Windows.Services;

public class DesktopIntegrationServiceImplementation : DesktopIntegrationService.DesktopIntegrationServiceBase
{

	private static readonly Logger Log = LogManager.GetLogger(nameof(DesktopIntegrationServiceImplementation));

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

	public override Task<ToggleMuteReply> ToggleMute(ToggleMuteRequest request, ServerCallContext context)
	{
		Log.Info("Executing [{Name}]", nameof(ToggleMute));
		var muteState = SimpleAudioManager.GetMasterVolumeMute();
		try
		{
			SimpleAudioManager.SetMasterVolumeMute(!muteState);
			return Task.FromResult(new ToggleMuteReply() { Muted = !muteState });
		}
		catch (Exception e)
		{
			Log.Error(e, "ToggleMute failed.");
			SimpleAudioManager.SetMasterVolumeMute(!muteState);
			return Task.FromResult(new ToggleMuteReply() { Muted = muteState });
		}
	}

	public override Task<SetMasterVolumeReply> SetMasterVolume(SetMasterVolumeRequest request, ServerCallContext context)
	{
		Log.Info("Executing [{Name}] [{NewValue}]", nameof(SetMasterVolume), request.Value);
		var previousVolume = SimpleAudioManager.GetMasterVolume();
		var newVolume = Math.Max(Math.Min(100, request.Value), 0);
		if (MathHelper.IsEqual(newVolume, previousVolume, 1.01f))
		{
			return Task.FromResult(new SetMasterVolumeReply() { Value = (int)previousVolume });
		}
		else
		{
			Log.Debug("Changing volume from [{From}] to [{To}]", previousVolume, newVolume);
			SimpleAudioManager.SetMasterVolume(newVolume);
			var masterVolume = SimpleAudioManager.GetMasterVolume();
			return Task.FromResult(new SetMasterVolumeReply() { Value = (int)masterVolume });
		}
	}

	public override Task<GetMasterVolumeReply> GetMasterVolume(GetMasterVolumeRequest request, ServerCallContext context)
	{
		Log.Info("Executing [{Name}]", nameof(GetMasterVolume));
		return Task.FromResult(new GetMasterVolumeReply() { Value = (int)SimpleAudioManager.GetMasterVolume() });
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

	public override async Task<GetClipboardResponse> GetClipboard(GetClipboardRequest request, ServerCallContext context)
	{
		Log.Info("Executing [{Name}]", nameof(GetClipboard));
		System.Windows.Application.Current.MainWindow?.Focus();
		if (MessageBox.Show($"Send clipboard content to {request.Requestee}?", "PC Remote 2", MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			try
			{
				var content = await ClipboardHelper.GetClipboardAsync(System.Windows.Forms.TextDataFormat.UnicodeText);

				Log.Debug("Returning {Length} characters to client", content.Length);
				return new GetClipboardResponse() { Content = content };
			}
			catch (Exception e)
			{
				Log.Error(e, "Failed to read clipboard.");
				return new GetClipboardResponse() { Content = default };
			}
		}
		else
		{
			Log.Warn("Permission denied");
			throw new RpcException(new Status(StatusCode.PermissionDenied, "Host declined permission"));
		}
	}

	public override async Task<SetClipboardResponse> SetClipboard(SetClipboardRequest request, ServerCallContext context)
	{
		Log.Info("Executing [{Name}]", nameof(GetClipboard));
		System.Windows.Application.Current.MainWindow?.Focus();
		if (MessageBox.Show($"Allow {request.Requestee} to set clipboard?", "PC Remote 2", MessageBoxButtons.YesNo) == DialogResult.Yes)
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

	public override async Task<SendMouseMoveResponse> SendMouseMove(IAsyncStreamReader<SendMouseMoveRequestItem> requestStream, ServerCallContext context)
	{
		Log.Debug("Sending mouse moves");
		try
		{
			while (await requestStream.MoveNext(context.CancellationToken))
			{
				var x = requestStream.Current.X;
				var y = requestStream.Current.Y;
				NativeMethods.Mouse.Move(x, y);

				Log.Trace("Mouse move: {X} {Y}", x, y);
			}
		}
		catch (OperationCanceledException e)
		{
			Log.Trace(e, "Operation cancelled");
		}
		catch (Exception e)
		{
			Log.Error(e, "An error occured while sending mouse input");
		}

		Log.Debug("Sending mouse moves done");

		return new SendMouseMoveResponse();
	}

	public override Task<DefaultResponse> SendLeftMouseButtonClick(DefaultRequest request, ServerCallContext context)
	{
		NativeMethods.Mouse.ClickLeft();
		return Task.FromResult(new DefaultResponse());
	}

	public override Task<DefaultResponse> SendRightMouseButtonClick(DefaultRequest request, ServerCallContext context)
	{
		NativeMethods.Mouse.ClickRight();
		return Task.FromResult(new DefaultResponse());
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
}