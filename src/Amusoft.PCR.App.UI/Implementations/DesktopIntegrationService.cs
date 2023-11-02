using System.Net;
using Amusoft.PCR.Application.Features.DesktopIntegration;
using Amusoft.PCR.Int.IPC;
using Grpc.Core;
using Grpc.Net.Client;

namespace Amusoft.PCR.App.UI.Implementations;

public class DesktopIntegrationService : IDesktopIntegrationService
{
	private readonly VoiceCommandService.VoiceCommandServiceClient _voiceCommandClient;

	private readonly Int.IPC.DesktopIntegrationService.DesktopIntegrationServiceClient _desktopClient;

	public DesktopIntegrationService(string protocol, IPEndPoint address)
	{
		var target = $"{protocol}://{address}";
		var channel = GrpcChannel.ForAddress(target);

		_voiceCommandClient = new VoiceCommandService.VoiceCommandServiceClient(channel);
		_desktopClient = new Int.IPC.DesktopIntegrationService.DesktopIntegrationServiceClient(channel);
		DesktopClient = new DesktopClientAdapter(_desktopClient);
	}

	public IDesktopIntegrationServiceClient DesktopClient { get; }
}