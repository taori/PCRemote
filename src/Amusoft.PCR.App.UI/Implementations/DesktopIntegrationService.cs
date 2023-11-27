using Amusoft.PCR.AM.Shared.Interfaces;
using Amusoft.PCR.AM.Shared.Services;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Int.IPC;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.App.UI.Implementations;

public class DesktopIntegrationService : IDesktopIntegrationService
{
	private readonly VoiceCommandService.VoiceCommandServiceClient _voiceCommandClient;

	public DesktopIntegrationService(GrpcChannel channel, IServiceProvider serviceProvider)
	{
		_voiceCommandClient = new VoiceCommandService.VoiceCommandServiceClient(channel);
		var desktopClient = new Int.IPC.DesktopIntegrationService.DesktopIntegrationServiceClient(channel);
		DesktopClient = new DesktopServiceClientWrapper(desktopClient, serviceProvider.GetRequiredService<ILogger<DesktopServiceClientWrapper>>());
	}

	public IDesktopClientMethods DesktopClient { get; }
}