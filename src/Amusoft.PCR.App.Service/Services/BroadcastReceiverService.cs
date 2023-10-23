using System.Text;
using Amusoft.Toolkit.Networking;

namespace Amusoft.PCR.App.Service.Services;

public class BroadcastReceiverService : BackgroundService
{
	private readonly ILogger<BroadcastReceiverService> _logger;

	private UdpBroadcastCommunicationChannel _channel = new(new UdpBroadcastCommunicationChannelSettings(50001));

	public BroadcastReceiverService(ILogger<BroadcastReceiverService> logger)
	{
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_channel.MessageReceived
			.Subscribe(result => _logger.LogInformation(Encoding.UTF8.GetString(result.Buffer)));
		await _channel.StartListeningAsync(stoppingToken);
	}
}