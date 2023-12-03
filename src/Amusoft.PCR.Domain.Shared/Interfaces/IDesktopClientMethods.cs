using Amusoft.PCR.Domain.Shared.Entities;
using Amusoft.PCR.Domain.Shared.ValueTypes;

namespace Amusoft.PCR.Domain.Shared.Interfaces;

public interface IDesktopClientMethods
{
	Task<bool?> SetMonitorBrightness(string id, int value);
	Task<Result<List<MonitorData>>> GetMonitorBrightness();
	Task<bool?> MonitorOn();
	Task<bool?> MonitorOff();
	Task<bool?> LockWorkStation();

	/// <summary>
	/// See https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.sendkeys?redirectedfrom=MSDN&view=net-5.0
	/// </summary>
	/// <param name="keys"></param>
	Task<bool?> SendKeys(string keys);
	Task<bool?> Shutdown(TimeSpan delay, bool force);
	Task<bool?> AbortShutdown();
	Task<bool?> Hibernate();
	Task<bool?> Restart(TimeSpan delay, bool force);
	Task<Result<List<ProcessData>>> GetProcessList();
	Task<bool?> KillProcessById(int processId);
	Task<bool?> FocusProcessWindow(int processId);
	Task<bool?> LaunchProgram(string programName, string? arguments = default);
	Task<bool?> SendMediaKey(MediaKeyCode code);
	Task<string?> GetClipboardAsync(string requestee);
	Task<bool?> SetClipboardAsync(string requestee, string content);
	Task<bool?> SendMouseMoveAsync(int x, int y);
	Task<bool?> SendLeftMouseClickAsync();
	Task<bool?> SendRightMouseClickAsync();
	Task<Result<List<AudioFeedData>>> GetAudioFeeds();
	Task<bool?> UpdateAudioFeed(AudioFeedData data);
	Task<Result<string>> SetUserPassword(string userName);
	Task<bool?> SuicideOnProcessExit(int processId);
}