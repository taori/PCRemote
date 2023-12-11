using System.Runtime.CompilerServices;
using Amusoft.PCR.Domain.Shared.Entities;
using Amusoft.PCR.Domain.Shared.Interfaces;
using Amusoft.PCR.Domain.Shared.ValueTypes;
using Amusoft.PCR.Int.IPC.Extensions;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.IPC.Integration;

public partial class DesktopServiceClientWrapper : IDesktopClientMethods
{
	private readonly ILogger<DesktopServiceClientWrapper> _logger;
	private readonly DesktopIntegrationService.DesktopIntegrationServiceClient _service;

	public DesktopServiceClientWrapper(DesktopIntegrationService.DesktopIntegrationServiceClient service, ILogger<DesktopServiceClientWrapper> logger)
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
			LogGenericMethodCallError(_logger, e);
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
			LogGenericMethodCallError(_logger, e);
			return default;
		}
	}

	public async Task<Result<List<MonitorData>>> GetMonitorBrightness()
	{
		try
		{
			_logger.LogDebug("{Method}()", nameof(GetMonitorBrightness));
			var monitors = await _service.GetMonitorBrightnessAsync(new GetMonitorBrightnessRequest());
			return Result.Success(monitors.Items.Select(d => d.ToDomainItem()).ToList());
		}
		catch (Exception e)
		{
			LogGenericMethodCallError(_logger, e);
			return Result.Error<List<MonitorData>>();
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
			LogGenericMethodCallError(_logger, e);
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
			LogGenericMethodCallError(_logger, e);
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
			LogGenericMethodCallError(_logger, e);
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
			LogGenericMethodCallError(_logger, e);
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
			LogGenericMethodCallError(_logger, e);
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

	public async Task<Result<List<ProcessData>>> GetProcessList()
	{
		try
		{
			_logger.LogDebug("{Method}", nameof(GetProcessList));
			var reply = await _service.GetProcessListAsync(new ProcessListRequest());
			return Result.Success(reply.Results.Select(d => d.ToDomainItem()).ToList());
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Exception occured while calling [{Name}]", nameof(GetProcessList));
			return Result.Error<List<ProcessData>>();
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

	public async Task<bool?> SendMediaKey(MediaKeyCode code)
	{
		try
		{
			_logger.LogDebug("{Method}({Code})", nameof(SendMediaKey), code);
			var reply = await _service.SendMediaKeysAsync(new SendMediaKeysRequest()
			{
				KeyCode = code.ToGrpcType()
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
			LogGenericMethodCallError(_logger, e);
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
			LogGenericMethodCallError(_logger, e);
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
			LogGenericMethodCallError(_logger, e);
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
			LogGenericMethodCallError(_logger, e);
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
			LogGenericMethodCallError(_logger, e);
			return default;
		}
	}

	public async Task<Result<List<AudioFeedData>>> GetAudioFeeds()
	{
		try
		{
			_logger.LogDebug("{Method}", nameof(GetAudioFeeds));
			var result = await _service.GetAudioFeedsAsync(new AudioFeedRequest());
			return result.Success
				? Result.Success(result.Items.Select(d => d.ToDomainItem()).ToList())
				: Result.Error<List<AudioFeedData>>();
		}
		catch (Exception e)
		{
			LogGenericMethodCallError(_logger, e);
			return Result.Error<List<AudioFeedData>>();
		}
	}

	public async Task<bool?> UpdateAudioFeed(AudioFeedData data)
	{
		try
		{
			_logger.LogDebug("{Method}", nameof(UpdateAudioFeed));
			var result = await _service.UpdateAudioFeedAsync(new UpdateAudioFeedRequest(){Item = data.ToGrpcItem()});
			return result.Success;
		}
		catch (Exception e)
		{
			LogGenericMethodCallError(_logger, e);
			return default;
		}
	}

	public async Task<Result<string>> SetUserPassword(string userName)
	{
		try
		{
			_logger.LogDebug("{Method}", nameof(SetUserPassword));
			var result = await _service.SetUserPasswordAsync(new ChangeUserPasswordRequest(){UserName = userName});
			return result.Success ? Result.Success(result.Content) : Result.Error<string>();
		}
		catch (Exception e)
		{
			LogGenericMethodCallError(_logger, e);
			return Result.Error<string>();
		}
	}

	[LoggerMessage(Level = LogLevel.Error, Message = "Exception occured while calling {Name}")]
	private static partial void LogGenericMethodCallError(ILogger logger, Exception exception, [CallerMemberName] string? name = default);
}