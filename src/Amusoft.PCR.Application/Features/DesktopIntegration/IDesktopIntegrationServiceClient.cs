using Amusoft.PCR.Domain.Common;
using Amusoft.PCR.Int.IPC;

namespace Amusoft.PCR.Application.Features.DesktopIntegration;

public interface IDesktopIntegrationServiceClient
{
	Task<bool> ShutDownDelayedAsync(bool force, int seconds);
	Task<Result<AudioFeedResponseItem[]>> GetAudioFeedsAsync();
	Task<bool> FocusWindowAsync(int processId);
	Task<bool> CheckIsAuthenticatedAsync();
	Task<bool> AuthenticateAsync();
	Task<bool> AbortShutDownAsync();
	Task<string> GetHostNameAsync();
}