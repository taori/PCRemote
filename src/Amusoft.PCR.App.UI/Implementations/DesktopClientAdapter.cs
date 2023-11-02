using Amusoft.PCR.Application.Features.DesktopIntegration;
using Amusoft.PCR.Int.IPC;
using Amusoft.PCR.Domain.Common;

namespace Amusoft.PCR.App.UI.Implementations;


public class DesktopClientAdapter : IDesktopIntegrationServiceClient
{
	private readonly Int.IPC.DesktopIntegrationService.DesktopIntegrationServiceClient _client;

	public DesktopClientAdapter(Int.IPC.DesktopIntegrationService.DesktopIntegrationServiceClient client)
	{
		_client = client;
	}

	public async Task Method() { }

	public async Task<bool> ShutDownDelayedAsync(bool force, int seconds)
	{
		var response = await _client.ShutDownDelayedAsync(new ShutdownDelayedRequest()
		{
			Force = force,
			Seconds = seconds
		});

		return response.Success;
	}

	public async Task<Result<AudioFeedResponseItem[]>> GetAudioFeedsAsync()
	{
		var response = await _client.GetAudioFeedsAsync(new AudioFeedRequest());
		return response.Success
			? Result.Success(response.Items.ToArray())
			: Result.Error<AudioFeedResponseItem[]>();
	}

	public async Task<bool> FocusWindowAsync(int processId)
	{
		var response = await _client.FocusWindowAsync(new FocusWindowRequest(){ ProcessId = processId});
		return response.Success;
	}

	public async Task<bool> CheckIsAuthenticatedAsync()
	{
		var response = await _client.CheckIsAuthenticatedAsync(new CheckIsAuthenticatedRequest());
		return response.Result;
	}

	public async Task<bool> AuthenticateAsync()
	{
		var response = await _client.AuthenticateAsync(new AuthenticateRequest());
		return response.Success;
	}

	public async Task<bool> AbortShutDownAsync()
	{
		var response = await _client.AbortShutDownAsync(new AbortShutdownRequest());
		return response.Success;
	}

	public async Task<string> GetHostNameAsync()
	{
		var response = await _client.GetHostNameAsync(new GetHostNameRequest());
		return response.Content;
	}
}