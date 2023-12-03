using System.Net;
using System.Net.Sockets;

namespace Amusoft.PCR.AM.Service.Interfaces;

public interface IBroadcastCommunicator : IDisposable
{
	IObservable<UdpReceiveResult> MessageReceived { get; }
	Task StartListeningAsync(CancellationToken stoppingToken);
	Task SendToAsync(byte[] bytes, IPEndPoint endpoint, CancellationToken cancellationToken);
}