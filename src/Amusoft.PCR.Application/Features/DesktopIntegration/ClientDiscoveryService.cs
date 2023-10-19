using Amusoft.PCR.Domain.AgentSettings;
using Amusoft.Toolkit.Networking;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Options;

namespace Amusoft.PCR.Application.Features.DesktopIntegration;

public class ClientDiscoveryService : IDisposable
{
	private readonly ILogger<ClientDiscoveryService> _logger;
	private readonly ServerUrlTransmitterSettings _settings;
	private UdpBroadcastCommunicationChannel _channel;

	public ClientDiscoveryService(ILogger<ClientDiscoveryService> logger, IOptions<ApplicationSettings> settings)
	{
		_logger = logger;
		_settings = settings.Value.ServerUrlTransmitter ?? throw new ArgumentNullException(nameof(settings.Value.ServerUrlTransmitter));
		var channelSettings = new UdpBroadcastCommunicationChannelSettings(_settings.Port);
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
			_logger.LogInformation("{Service} is listening on port {Port}", nameof(ClientDiscoveryService), _settings.Port);
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
		var ports = _settings.PublicHttpsPorts;
		if (ports != null)
		{
			var replyText = GrpcHandshakeFormatter.Write(Environment.MachineName, ports);
			await _channel.SendToAsync(Encoding.UTF8.GetBytes(replyText), received.RemoteEndPoint);
			_logger.LogDebug("Reply \"{Message}\" sent to {Address}", replyText, received.RemoteEndPoint.Address.ToString());
		}
	}
}