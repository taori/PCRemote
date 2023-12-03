using Amusoft.PCR.AM.Service.Interfaces;
using Amusoft.PCR.Domain.Service.Entities;
using Amusoft.Toolkit.Networking;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.Service.Services;

internal class BroadcastCommunicatorFactory : IBroadcastCommunicatorFactory
{
	private readonly ILogger<BroadcastCommunicatorFactory> _logger;

	public BroadcastCommunicatorFactory(ILogger<BroadcastCommunicatorFactory> logger)
	{
		_logger = logger;
	}
	
	public IBroadcastCommunicator Create(ServerUrlTransmitterSettings settings)
	{
		_logger.LogDebug("Creating Broadcast Communicator for Handshake Port {Port}", settings.HandshakePort);
		var channelSettings = new UdpBroadcastCommunicationChannelSettings(settings.HandshakePort);
		channelSettings.AllowNatTraversal = true;

		var communicator = new BroadcastCommunicator(channelSettings);
		return communicator;
	}
}