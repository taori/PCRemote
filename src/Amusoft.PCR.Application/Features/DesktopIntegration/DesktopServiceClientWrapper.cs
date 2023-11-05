using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Domain.Common;
using Amusoft.PCR.Int.IPC;
using Google.Protobuf.Collections;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Application.Features.DesktopIntegration;

public class DesktopServiceClientWrapper : IDesktopClientMethods
{
	private readonly ILogger<DesktopServiceClientWrapper> _logger;
	private readonly Int.IPC.DesktopIntegrationService.DesktopIntegrationServiceClient _service;

	public DesktopServiceClientWrapper(Int.IPC.DesktopIntegrationService.DesktopIntegrationServiceClient service, ILogger<DesktopServiceClientWrapper> logger)
	{
		_logger = logger;
		_service = service;
	}

	public async Task<bool?> ToggleMute()
	{
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(ToggleMute));
			var reply = await _service.ToggleMuteAsync(new ToggleMuteRequest());
			return reply.Muted;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}]", nameof(ToggleMute));
			return default;
		}
	}

	public async Task<bool?> MonitorOn()
	{
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(MonitorOn));
			await _service.MonitorOnAsync(new MonitorOnRequest());
			return true;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}]", nameof(MonitorOn));
			return default;
		}
	}

	public async Task<bool?> MonitorOff()
	{
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(MonitorOff));
			await _service.MonitorOffAsync(new MonitorOffRequest());
			return true;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}]", nameof(MonitorOff));
			return default;
		}
	}

	public async Task<bool?> LockWorkStation()
	{
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(LockWorkStation));
			await _service.LockWorkStationAsync(new LockWorkStationRequest());
			return true;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}]", nameof(LockWorkStation));
			return default;
		}
	}

	public async Task<bool?> SendKeys(string keys)
	{
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(SendKeys));
			await _service.SendKeysAsync(new SendKeysRequest() { Message = keys });
			return true;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}] with [{Keys}]", nameof(SendKeys), keys);
			return default;
		}
	}

	public async Task<int?> SetMasterVolume(int value)
	{
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(SetMasterVolume));
			var reply = await _service.SetMasterVolumeAsync(new SetMasterVolumeRequest() { Value = value });
			return reply.Value;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}] to [{Value}]", nameof(SetMasterVolume), value);
			return default;
		}
	}

	public async Task<int?> GetMasterVolume()
	{
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(GetMasterVolume));
			var reply = await _service.GetMasterVolumeAsync(new GetMasterVolumeRequest());
			return reply.Value;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}]", nameof(GetMasterVolume));
			return -1;
		}
	}

	public async Task<bool?> Shutdown(TimeSpan delay, bool force)
	{
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(Shutdown));
			var reply = await _service.ShutDownDelayedAsync(new ShutdownDelayedRequest() { Seconds = (int)delay.TotalSeconds, Force = force });
			return reply.Success;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}] with delay [{Delay}] and force close [{Force}]", nameof(Shutdown), delay, force);
			return default;
		}
	}

	public async Task<bool?> AbortShutdown()
	{
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(AbortShutdown));
			var reply = await _service.AbortShutDownAsync(new AbortShutdownRequest());
			return reply.Success;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}]", nameof(AbortShutdown));
			return default;
		}
	}

	public async Task<bool?> Hibernate()
	{
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(Hibernate));
			var reply = await _service.HibernateAsync(new HibernateRequest());
			return reply.Success;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}]", nameof(Hibernate));
			return default;
		}
	}

	public async Task<bool?> Restart(TimeSpan delay, bool force)
	{
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(Restart));
			var reply = await _service.RestartAsync(new RestartRequest() { Delay = (int)delay.TotalSeconds, Force = force });
			return reply.Success;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}] with delay [{Delay}] and force close [{Force}]", nameof(Restart), delay, force);
			return default;
		}
	}

	private static readonly List<ProcessListResponseItem> EmptyProcessList = new();

	public async Task<Result<IList<ProcessListResponseItem>>> GetProcessList()
	{
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(GetProcessList));
			var reply = await _service.GetProcessListAsync(new ProcessListRequest());
			return Result.Success(reply.Results as IList<ProcessListResponseItem>);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}]", nameof(GetProcessList));
			return Result.Error<IList<ProcessListResponseItem>>();
		}
	}

	public async Task<bool?> KillProcessById(int processId)
	{
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(KillProcessById));
			var reply = await _service.KillProcessByIdAsync(new KillProcessRequest() { ProcessId = processId });
			return reply.Success;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}] with id [{Id}]", nameof(KillProcessById), processId);
			return default;
		}
	}

	public async Task<bool?> FocusProcessWindow(int processId)
	{
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(FocusProcessWindow));
			var reply = await _service.FocusWindowAsync(new FocusWindowRequest() { ProcessId = processId });
			return reply.Success;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}] with id [{Id}]", nameof(FocusProcessWindow), processId);
			return default;
		}
	}

	public async Task<bool?> LaunchProgram(string programName, string? arguments = null)
	{
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(LaunchProgram));
			var request = string.IsNullOrEmpty(arguments)
				? new LaunchProgramRequest() { ProgramName = programName }
				: new LaunchProgramRequest() { ProgramName = programName, Arguments = arguments };

			var reply = await _service.LaunchProgramAsync(request);
			return reply.Success;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}] [{ProgramName}] with arguments [{Arguments}]", nameof(LaunchProgram), programName, arguments);
			return default;
		}
	}

	public async Task<bool?> SendMediaKey(SendMediaKeysRequest.Types.MediaKeyCode code)
	{
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(SendMediaKey));
			var reply = await _service.SendMediaKeysAsync(new SendMediaKeysRequest()
			{
				KeyCode = code
			});
			return reply.Success;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}] with code [{Code}]", nameof(SendMediaKey), code);
			return default;
		}
	}

	public async Task<string?> GetClipboardAsync(string requestee)
	{
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(GetClipboardAsync));
			var content = await _service.GetClipboardAsync(new GetClipboardRequest() { Requestee = requestee });
			return content.Content;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}]", nameof(GetClipboardAsync));
			return default;
		}
	}

	public async Task<bool?> SetClipboardAsync(string requestee, string content)
	{
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(SetClipboardAsync));
			var result = await _service.SetClipboardAsync(new SetClipboardRequest() { Content = content, Requestee = requestee });
			return result.Success;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}]", nameof(SetClipboardAsync));
			return default;
		}
	}

	public async Task SendMouseMoveAsync(IAsyncStreamReader<SendMouseMoveRequestItem> streamReader, CancellationToken cancellationToken)
	{
		var mouseStream = _service.SendMouseMove();
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(SendMouseMoveAsync));

			while (await streamReader.MoveNext(cancellationToken))
			{
				await mouseStream.RequestStream.WriteAsync(streamReader.Current);
			}

			_logger.LogInformation("{Method} terminated", nameof(SendMouseMoveAsync));
		}
		catch (OperationCanceledException e)
		{
			_logger.LogTrace(e, "Operation cancelled");
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}]", nameof(SendMouseMoveAsync));
		}
		finally
		{
			await mouseStream.RequestStream.CompleteAsync();
			mouseStream.Dispose();
		}
	}

	public async Task<bool?> SendLeftMouseClickAsync()
	{
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(SendLeftMouseClickAsync));
			var result = await _service.SendLeftMouseButtonClickAsync(new DefaultRequest());
			return result.Success;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}]", nameof(SendLeftMouseClickAsync));
			return default;
		}
	}

	public async Task<bool?> SendRightMouseClickAsync()
	{
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(SendRightMouseClickAsync));
			var result = await _service.SendRightMouseButtonClickAsync(new DefaultRequest());
			return result.Success;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}]", nameof(SendRightMouseClickAsync));
			return default;
		}
	}

	public async Task<AudioFeedResponse?> GetAudioFeedsResponse()
	{
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(GetAudioFeedsResponse));
			var result = await _service.GetAudioFeedsAsync(new AudioFeedRequest());
			return result;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}]", nameof(GetAudioFeedsResponse));
			return default;
		}
	}

	public async Task<DefaultResponse?> UpdateAudioFeed(UpdateAudioFeedRequest request)
	{
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(UpdateAudioFeed));
			var result = await _service.UpdateAudioFeedAsync(request);
			return result;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}]", nameof(UpdateAudioFeed));
			return default;
		}
	}

	public async Task<StringResponse?> SetUserPassword(ChangeUserPasswordRequest request)
	{
		try
		{
			_logger.LogInformation("Calling method {Method}", nameof(SetUserPassword));
			var result = await _service.SetUserPasswordAsync(request);
			return result;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}]", nameof(SetUserPassword));
			return default;
		}
	}
}