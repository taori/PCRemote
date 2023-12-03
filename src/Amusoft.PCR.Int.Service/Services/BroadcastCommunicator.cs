using System.Net;
using System.Net.Sockets;
using Amusoft.PCR.AM.Service.Interfaces;
using Amusoft.Toolkit.Networking;

namespace Amusoft.PCR.Int.Service.Services;

internal class BroadcastCommunicator : IBroadcastCommunicator
{
	private readonly UdpBroadcastCommunicationChannel _channel;

	public BroadcastCommunicator(UdpBroadcastCommunicationChannelSettings settings)
	{
		_channel = new UdpBroadcastCommunicationChannel(settings);
	}

	public void Dispose()
	{
		_channel.Dispose();
	}

	public IObservable<UdpReceiveResult> MessageReceived => _channel.MessageReceived;
	
	public Task StartListeningAsync(CancellationToken stoppingToken)
	{
		return _channel.StartListeningAsync(stoppingToken);
	}

	public Task SendToAsync(byte[] bytes, IPEndPoint endpoint, CancellationToken cancellationToken)
	{
		return _channel.SendToAsync(bytes, endpoint, cancellationToken);
	}
}