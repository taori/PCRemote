using System.Collections.Immutable;
using Amusoft.PCR.AM.Agent.Interfaces;
using Amusoft.PCR.Domain.Agent.Entities;
using Amusoft.PCR.Domain.Agent.ValueTypes;
using Amusoft.PCR.Domain.Shared.Entities;
using Amusoft.PCR.Int.Agent.Services;
using Amusoft.PCR.Int.WindowsAgent.Interop;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.WindowsAgent.Dependencies;

public class DesktopIntegrationProcessor : IDesktopIntegrationProcessor
{
	private readonly ILogger<DesktopIntegrationProcessor> _logger;

	private NativeMonitorManager _monitorManager = new();

	public DesktopIntegrationProcessor(ILogger<DesktopIntegrationProcessor> logger)
	{
		_logger = logger;
	}
	
	public bool TrySetMonitorBrightness(string monitorId, int value)
	{
		try
		{
			if (_monitorManager.Monitors.FirstOrDefault(d => d.PhysicalMonitorHandle.ToString().Equals(monitorId)) is {} match)
			{
				_monitorManager.SetBrightness(match, (uint)value);
				return true;
			}

			return false;
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(TrySetMonitorBrightness));
			return false;
		}
	}

	public bool TryGetMonitors(out ICollection<MonitorData> monitors)
	{
		try
		{
			monitors = _monitorManager.Monitors.Select(d => new MonitorData(
				d.PhysicalMonitorHandle.ToString(),
				d.Description,
				d.CurrentValue,
				d.MinValue,
				d.MaxValue
			)).ToList();

			return true;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to access monitors");
			monitors = ImmutableList<MonitorData>.Empty;
			return false;
		}
	}

	public bool TryListenForProcessExit(int processId)
	{
		return ProcessExitListenerManager.TryObserveProcessExit(processId);
	}

	public bool TryMonitorsOn()
	{
		try
		{
			NativeMethods.Monitor.On();
			return true;
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(TryMonitorsOn));
			return false;
		}
	}

	public bool TryMonitorsOff()
	{
		try
		{
			NativeMethods.Monitor.Off();
			return true;
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(TryMonitorsOff));
			return false;
		}
	}

	public bool TryAbortShutdown()
	{
		return MachineStateHelper.TryAbortShutDown();
	}

	public bool TryShutdown(int shutdownInSeconds, bool forceShutdown)
	{
		return MachineStateHelper.TryShutDownDelayed(TimeSpan.FromSeconds(shutdownInSeconds), forceShutdown);
	}

	public bool TryRestart(int restartInSeconds, bool forceRestart)
	{
		return MachineStateHelper.TryRestart(TimeSpan.FromSeconds(restartInSeconds), forceRestart);
	}

	public bool TryHibernate()
	{
		return MachineStateHelper.TryHibernate();
	}

	public bool TrySendKeys(string keySequence)
	{
		try
		{
			NativeMethods.SendKeys(keySequence);
			return true;
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(TrySendKeys));
			return false;
		}
	}

	public bool TrySendMediaKeys(MediaKeyCode code)
	{
		try
		{
			switch (code)
			{
				case MediaKeyCode.NextTrack:
					NativeMethods.PressMediaKey(NativeMethods.MediaKeyCode.NextTrack);
					break;
				case MediaKeyCode.PreviousTrack:
					NativeMethods.PressMediaKey(NativeMethods.MediaKeyCode.PreviousTrack);
					break;
				case MediaKeyCode.PlayPause:
					NativeMethods.PressMediaKey(NativeMethods.MediaKeyCode.PlayPause);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			return true;
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(TrySendKeys));
			return false;
		}
	}

	public bool TryLockWorkstation()
	{
		try
		{
			NativeMethods.LockWorkStation();
			return true;
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(TryLockWorkstation));
			return false;
		}
	}

	public bool TryGetProcessList(out ICollection<ProcessData> processes)
	{
		if (!ProcessHelper.TryGetProcessList(out var processList))
		{
			processes = ArraySegment<ProcessData>.Empty;
			return false;
		}

		var l = new List<ProcessData>();
		l.AddRange(processList);
		processes = l;
		return true;
	}

	public bool TryKillProcessById(int processId)
	{
		return ProcessHelper.TryKillProcess(processId);
	}

	public async Task<Result<string>> TryGetClipboardAsync()
	{
		try
		{
			var content = await ClipboardHelper.GetClipboardAsync(TextDataFormat.UnicodeText);
			_logger.LogDebug("Returning {Length} characters to client", content.Length);
			return Result.Success(content);
		}
		catch (Exception e)
		{
			_logger.LogDebug(e, "Failed to read clipboard.");
			return Result.Error<string>();
		}
	}

	public async Task<bool> TrySetClipboardAsync(string content)
	{
		try
		{
			await ClipboardHelper.SetClipboardAsync(content, TextDataFormat.UnicodeText);
			_logger.LogDebug("Setting {Length} characters to client", content.Length);
			return true;
		}
		catch (Exception e)
		{
			_logger.LogDebug(e, "Failed to set clipboard.");
			return false;
		}
	}

	public bool TrySendMouseMove(int x, int y)
	{
		try
		{
			NativeMethods.Mouse.Move(x, y);
			return true;
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(TrySendMouseMove));
			return false;
		}
	}

	public bool TrySendMouseLeftButtonClick()
	{
		try
		{
			NativeMethods.Mouse.ClickLeft();
			return true;
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(TrySendMouseLeftButtonClick));
			return false;
		}
	}

	public bool TrySendMouseRightButtonClick()
	{
		try
		{
			NativeMethods.Mouse.ClickRight();
			return true;
		}
		catch (Exception e)
		{
			_logger.LogError(e, nameof(TrySendMouseRightButtonClick));
			return false;
		}
	}

	public bool TryGetAudioFeeds(out ICollection<AudioFeedData> feeds)
	{
		if (SimpleAudioManager.TryGetAudioFeeds(out var f))
		{
			feeds = new List<AudioFeedData>(f);
			return true;
		}
		else
		{
			feeds = ArraySegment<AudioFeedData>.Empty;
			return false;
		}
	}

	public bool TryUpdateAudioFeed(AudioFeedData data)
	{
		return SimpleAudioManager.TryUpdateFeed(data);
	}
}