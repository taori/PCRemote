using System.Net;
using System.Net.Sockets;
using System.Text;
using Amusoft.PCR.AM.Service.Interfaces;
using Amusoft.PCR.Domain.Service.Entities;
using Amusoft.PCR.Int.IPC;
using Amusoft.Toolkit.Networking;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amusoft.PCR.AM.Service.Services;

public class ClientDiscoveryService : IDisposable
{
	private readonly ILogger<ClientDiscoveryService> _logger;
	private readonly IConnectedServerPorts _connectedServerPorts;
	private readonly ServerUrlTransmitterSettings _settings;
	private UdpBroadcastCommunicationChannel _channel;

	public ClientDiscoveryService(ILogger<ClientDiscoveryService> logger, IOptions<ApplicationSettings> settings, IConnectedServerPorts connectedServerPorts)
	{
		_logger = logger;
		_connectedServerPorts = connectedServerPorts;
		_settings = settings.Value.ServerUrlTransmitter ?? throw new ArgumentNullException(nameof(settings.Value.ServerUrlTransmitter));
		var channelSettings = new UdpBroadcastCommunicationChannelSettings(_settings.HandshakePort);
		channelSettings.AllowNatTraversal = true;

		_channel = new UdpBroadcastCommunicationChannel(channelSettings);
	}

	public void Dispose()
	{
		_channel?.Dispose();
	}

	public async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var receiveHandler = _channel.MessageReceived
			.Subscribe(async (d) => { await HandleReceive(d); });
		try
		{
			_logger.LogInformation("{Service} is listening on port {Port}, Known as {MachineName}", nameof(ClientDiscoveryService), _settings.HandshakePort, GetMachineName());
			await _channel.StartListeningAsync(stoppingToken);

			_logger.LogInformation("Channel terminated");
		}
		catch (OperationCanceledException)
		{
			receiveHandler.Dispose();
			_logger.LogInformation("Terminating channel");
		}
		catch (Exception e)
		{
			receiveHandler.Dispose();
			_logger.LogError(e, "Terminating channel");
		}
	}

	private async Task HandleReceive(UdpReceiveResult received)
	{
		var message = Encoding.UTF8.GetString(received.Buffer);
		if (!string.Equals(message, GrpcHandshakeClientMessage.Message))
		{
			_logger.LogDebug("Discarding message - invalid (Origin: {Origin})", received.RemoteEndPoint.Address.ToString());
			return;
		}

		_logger.LogInformation("Received handshake from [{Address}]", received.RemoteEndPoint);
		if (_connectedServerPorts.Addresses is {Count: > 0} ports)
		{
			var replyText = GrpcHandshakeFormatter.Write(GetMachineName(), ports.ToArray());
			if (await IsSameOriginMessageAsync(received.RemoteEndPoint))
			{
				await _channel.SendToAsync(Encoding.UTF8.GetBytes(replyText), new IPEndPoint(IPAddress.Broadcast, received.RemoteEndPoint.Port), CancellationToken.None);
			}
			else
			{
				await _channel.SendToAsync(Encoding.UTF8.GetBytes(replyText), received.RemoteEndPoint, CancellationToken.None);
			}

			_logger.LogDebug("Reply \"{Message}\" sent to {Address}", replyText, received.RemoteEndPoint.Address.ToString());
		}
	}

	private async Task<bool> IsSameOriginMessageAsync(IPEndPoint remoteEndPoint)
	{
		var addresses = await Dns.GetHostAddressesAsync(Dns.GetHostName());
		var isMatch = addresses.Any(d => d.Equals(remoteEndPoint.Address));
		return isMatch;
	}

	private string GetMachineName()
	{
		return _settings.HostAlias ?? Environment.MachineName;
	}
}