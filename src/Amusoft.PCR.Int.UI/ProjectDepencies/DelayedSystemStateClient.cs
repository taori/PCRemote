#region

using System.Net;
using Amusoft.PCR.Int.IPC;

#endregion

namespace Amusoft.PCR.Int.UI.ProjectDepencies;

public class DelayedSystemStateClient
{
	private readonly DesktopIntegrationService.DesktopIntegrationServiceClient _client;

	public DelayedSystemStateClient(string protocol, IPEndPoint endPoint)
	{
		// TODO this has to be fixed
		var channelFactory = new GrpcChannelFactory(null, null);
		var channel = channelFactory.Create(protocol, endPoint);
		_client = new DesktopIntegrationService.DesktopIntegrationServiceClient(channel);
	}

	public async Task ShutdownAsync(TimeSpan delay, bool force)
	{
		await _client.ShutDownDelayedAsync(new ShutdownDelayedRequest() { Force = force, Seconds = (int)delay.TotalSeconds })
			.ConfigureAwait(false);
	}

	public async Task RestartAsync(TimeSpan delay, bool force)
	{
		await _client.RestartAsync(new RestartRequest() { Force = force, Delay = (int)delay.TotalSeconds })
			.ConfigureAwait(false);
	}

	public async Task HibernateAsync(TimeSpan delay)
	{
		// hibernation is an immediate process by default and therefore requires a delay
		await Task.Delay(delay)
			.ConfigureAwait(false);
		_ = _client.HibernateAsync(new HibernateRequest());
	}
}