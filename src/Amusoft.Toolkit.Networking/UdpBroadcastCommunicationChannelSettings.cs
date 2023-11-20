namespace Amusoft.Toolkit.Networking;

public class UdpBroadcastCommunicationChannelSettings
{
	public UdpBroadcastCommunicationChannelSettings(int port)
	{
		if(port > Math.Pow(2,16))
			throw new ArgumentOutOfRangeException("port", $"Port number cannot be greater than {Math.Pow(2,16)}");
		Port = port;
	}

	public int Port { get; set; }

	public bool AllowNatTraversal { get; set; }

	public Action<Exception>? ReceiveErrorHandler { get; set; }
}