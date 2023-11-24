using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Domain.Common;
using Amusoft.PCR.Int.IPC;
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

	public async Task<bool?> SuicideOnProcessExit(int processId)
	{
		try
		{
			_logger.LogDebug("{Method}({Id})", nameof(SuicideOnProcessExit), processId);
			await _service.SuicideOnProcessExitAsync(new SuicideOnProcessExitRequest() {ProcessId = processId});
			return true;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}]", nameof(SuicideOnProcessExit));
			return default;
		}
	}

	public async Task<bool?> SetMonitorBrightness(string id, int value)
	{
		try
		{
			_logger.LogDebug("{Method}({Id}, {Value})", nameof(SetMonitorBrightness), id, value);
			var monitors = await _service.SetMonitorBrightnessAsync(new SetMonitorBrightnessRequest() {Id = id, Value = value});
			return monitors.Success;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}]", nameof(SetMonitorBrightness));
			return default;
		}
	}

	public async Task<Result<List<GetMonitorBrightnessResponseItem>>> GetMonitorBrightness()
	{
		try
		{
			_logger.LogDebug("{Method}()", nameof(GetMonitorBrightness));
			var monitors = await _service.GetMonitorBrightnessAsync(new GetMonitorBrightnessRequest());
			return Result.Success(monitors.Items.ToList());
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}]", nameof(GetMonitorBrightness));
			return Result.Error<List<GetMonitorBrightnessResponseItem>>();
		}
	}

	public async Task<bool?> MonitorOn()
	{
		try
		{
			_logger.LogDebug("{Method}", nameof(MonitorOn));
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
			_logger.LogDebug("{Method}", nameof(MonitorOff));
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
			_logger.LogDebug("{Method}", nameof(LockWorkStation));
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
			_logger.LogDebug("{Method}({Keys})", nameof(SendKeys), keys);
			await _service.SendKeysAsync(new SendKeysRequest() { Message = keys });
			return true;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}] with [{Keys}]", nameof(SendKeys), keys);
			return default;
		}
	}

	public async Task<bool?> Shutdown(TimeSpan delay, bool force)
	{
		try
		{
			_logger.LogDebug("{Method}({Delay}, {Force})", nameof(Shutdown), delay, force);
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
			_logger.LogDebug("{Method}", nameof(AbortShutdown));
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
			_logger.LogDebug("{Method}", nameof(Hibernate));
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
			_logger.LogDebug("{Method}({Delay}, {Force})", nameof(Restart), delay, force);
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
			_logger.LogDebug("{Method}", nameof(GetProcessList));
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
			_logger.LogDebug("{Method}({ProcessId})", nameof(KillProcessById), processId);
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
			_logger.LogDebug("{Method}({ProcessId})", nameof(FocusProcessWindow), processId);
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
			_logger.LogDebug("{Method}({ProgramName}, {Arguments})", nameof(LaunchProgram), programName, arguments);
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
			_logger.LogDebug("{Method}({Code})", nameof(SendMediaKey), code);
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
			_logger.LogDebug("{Method}({Requestee})", nameof(GetClipboardAsync), requestee);
			var response = await _service.GetClipboardAsync(new GetClipboardRequest() { Requestee = requestee });
			return response.Success ? response.Content : null;
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
			_logger.LogDebug("{Method}({Requestee}, ***)", nameof(SetClipboardAsync), requestee);
			var result = await _service.SetClipboardAsync(new SetClipboardRequest() { Content = content, Requestee = requestee });
			return result.Success;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}]", nameof(SetClipboardAsync));
			return default;
		}
	}

	public async Task<bool?> SendMouseMoveAsync(int x, int y)
	{
		try
		{
			// _logger.LogTrace("{Method}", nameof(SendMouseMoveAsync));
			var result = await _service.SendMouseMoveAsync(new SendMouseMoveRequest(){X = x, Y = y});
			return result.Success;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}]", nameof(SendMouseMoveAsync));
			return default;
		}
	}

	public async Task<bool?> SendLeftMouseClickAsync()
	{
		try
		{
			_logger.LogDebug("{Method}", nameof(SendLeftMouseClickAsync));
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
			_logger.LogDebug("{Method}", nameof(SendRightMouseClickAsync));
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
			_logger.LogDebug("{Method}", nameof(GetAudioFeedsResponse));
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
			_logger.LogDebug("{Method}", nameof(UpdateAudioFeed));
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
			_logger.LogDebug("{Method}", nameof(SetUserPassword));
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