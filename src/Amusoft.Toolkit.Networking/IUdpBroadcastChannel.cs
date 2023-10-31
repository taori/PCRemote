using System.Net;

namespace Amusoft.Toolkit.Networking;

public interface IUdpBroadcastChannel
{
	Task<bool> BroadcastAsync(byte[] bytes, CancellationToken cancellationToken);
	Task<bool> SendToAsync(byte[] bytes, IPEndPoint endPoint, CancellationToken cancellationToken);
}