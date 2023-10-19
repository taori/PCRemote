using System.Net.Sockets;
using System.Net;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;

namespace Amusoft.Toolkit.Networking;

public class UdpBroadcastCommunicationChannel : IDisposable
{
	private readonly UdpClient _client;
	private readonly UdpBroadcastCommunicationChannelSettings _settings;
	private CancellationTokenSource? _cts;

	private readonly Subject<UdpReceiveResult> _messageReceived = new();

	public IObservable<UdpReceiveResult> MessageReceived => _messageReceived;

	public UdpBroadcastCommunicationChannel(UdpBroadcastCommunicationChannelSettings settings)
	{
		_settings = settings;
		_client = new UdpClient()
		{
			EnableBroadcast = true,
			ExclusiveAddressUse = false
		};

		if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			if (_settings.AllowNatTraversal)
				_client.AllowNatTraversal(true);
		}

		_client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
		_client.Client.Bind(new IPEndPoint(IPAddress.Any, _settings.Port));
	}

	public async void StartListening(CancellationToken token)
	{
		await StartListeningAsync(token);
	}

	public async Task StartListeningAsync(CancellationToken token)
	{
		_cts?.Dispose();
		_cts = CancellationTokenSource.CreateLinkedTokenSource(token);
		await Task.Run(async () =>
		{
			try
			{
				while (!_cts.IsCancellationRequested)
				{
					var result = await _client.ReceiveAsync(_cts.Token);
					_messageReceived.OnNext(result);
				}
			}
			catch (OperationCanceledException)
			{
				_messageReceived.OnCompleted();
			}
			catch (Exception e)
			{
				_settings.ReceiveErrorHandler?.Invoke(e);
				_messageReceived.OnError(e);
			}
		}, _cts.Token);
	}

	public async Task<bool> SendAsync(byte[] bytes)
	{
		return await SendToAsync(bytes, new IPEndPoint(IPAddress.Broadcast, _settings.Port));
	}

	public async Task<bool> SendToAsync(byte[] bytes, IPEndPoint endPoint)
	{
		var byteLength = bytes.Length;
		return await _client.SendAsync(bytes, byteLength, endPoint) == byteLength;
	}

	private bool _disposed;
	public void Dispose()
	{
		if (_disposed)
			return;

		_cts?.Dispose();
		_client.Dispose();
		_disposed = true;
	}
}

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