using Amusoft.PCR.Domain.Agent.Entities;
using Amusoft.PCR.Domain.Agent.ValueTypes;
using Amusoft.PCR.Domain.Shared.Entities;

namespace Amusoft.PCR.AM.Agent.Interfaces;

public interface IDesktopIntegrationProcessor
{
	bool TrySetMonitorBrightness(string monitorId, int value);
	bool TryGetMonitors(out ICollection<MonitorData> monitors);
	bool TryListenForProcessExit(int processId);
	bool TryMonitorsOn();
	bool TryMonitorsOff();
	bool TryAbortShutdown();
	bool TryShutdown(int shutdownInSeconds, bool forceShutdown);
	bool TryRestart(int restartInSeconds, bool forceRestart);
	bool TryHibernate();
	bool TrySendKeys(string keySequence);
	bool TrySendMediaKeys(MediaKeyCode code);
	bool TryLockWorkstation();
	bool TryGetProcessList(out ICollection<ProcessData> processes);
	bool TryKillProcessById(int processId);
	Task<Result<string>> TryGetClipboardAsync();
	Task<bool> TrySetClipboardAsync(string content);
	bool TrySendMouseMove(int x, int y);
	bool TrySendMouseLeftButtonClick();
	bool TrySendMouseRightButtonClick();
	bool TryGetAudioFeeds(out ICollection<AudioFeedData> feeds);
	bool TryUpdateAudioFeed(AudioFeedData data);
}