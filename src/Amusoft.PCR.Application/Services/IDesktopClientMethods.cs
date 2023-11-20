using Amusoft.PCR.Domain.Common;
using Amusoft.PCR.Int.IPC;
using Grpc.Core;

namespace Amusoft.PCR.Application.Services;

public interface IDesktopClientMethods
{
	Task<bool?> SetMonitorBrightness(string id, int value);
	Task<Result<List<GetMonitorBrightnessResponseItem>>> GetMonitorBrightness();
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
	Task<Result<IList<ProcessListResponseItem>>> GetProcessList();
	Task<bool?> KillProcessById(int processId);
	Task<bool?> FocusProcessWindow(int processId);
	Task<bool?> LaunchProgram(string programName, string? arguments = default);
	Task<bool?> SendMediaKey(SendMediaKeysRequest.Types.MediaKeyCode code);
	Task<string?> GetClipboardAsync(string requestee);
	Task<bool?> SetClipboardAsync(string requestee, string content);
	Task<bool?> SendMouseMoveAsync(int x, int y);
	Task<bool?> SendLeftMouseClickAsync();
	Task<bool?> SendRightMouseClickAsync();
	Task<AudioFeedResponse?> GetAudioFeedsResponse();
	Task<DefaultResponse?> UpdateAudioFeed(UpdateAudioFeedRequest request);
	Task<StringResponse?> SetUserPassword(ChangeUserPasswordRequest request);
	Task<bool?> SuicideOnProcessExit(int processId);
}