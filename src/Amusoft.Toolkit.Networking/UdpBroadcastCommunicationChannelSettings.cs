namespace Amusoft.Toolkit.Networking;

public class UdpBroadcastCommunicationChannelSettings
{
	public UdpBroadcastCommunicationChannelSettings(int port)
	{
		Port = port;
	}

	public int Port { get; set; }

	public bool AllowNatTraversal { get; set; }

	public Action<Exception>? ReceiveErrorHandler { get; set; }
}