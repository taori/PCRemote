using System.Net;
using Amusoft.PCR.Int.IPC;
using Grpc.Net.Client;

namespace Amusoft.PCR.Int.UI.ProjectDepencies;

public class SystemStateClient
{
	private readonly DesktopIntegrationService.DesktopIntegrationServiceClient _client;

	public SystemStateClient(string protocol, IPEndPoint endPoint)
	{
		var channelFactory = new GrpcChannelFactory(null);
		var channel = channelFactory.Create(protocol, endPoint);
		_client = new DesktopIntegrationService.DesktopIntegrationServiceClient(channel);
	}

	public async Task ShutdownAsync(TimeSpan delay, bool force)
	{
		await _client.ShutDownDelayedAsync(new ShutdownDelayedRequest() { Force = force, Seconds = (int)delay.TotalSeconds });
	}

	public async Task RestartAsync(TimeSpan delay, bool force)
	{
		await _client.RestartAsync(new RestartRequest() { Force = force, Delay = (int)delay.TotalSeconds });
	}

	public Task HibernateAsync(TimeSpan delay, bool force)
	{
		_ = _client.HibernateAsync(new HibernateRequest());
		return Task.CompletedTask;
	}
}