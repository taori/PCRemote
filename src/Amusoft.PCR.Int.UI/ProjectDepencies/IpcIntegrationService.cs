﻿using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Domain.Shared.Interfaces;
using Amusoft.PCR.Int.IPC;
using Amusoft.PCR.Int.IPC.Integration;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.UI.ProjectDepencies;

public class IpcIntegrationService : IIpcIntegrationService
{
	private readonly VoiceCommandService.VoiceCommandServiceClient _voiceCommandClient;

	public IpcIntegrationService(GrpcChannel channel, IServiceProvider serviceProvider)
	{
		_voiceCommandClient = new VoiceCommandService.VoiceCommandServiceClient(channel);
		var desktopClient = new Int.IPC.DesktopIntegrationService.DesktopIntegrationServiceClient(channel);
		DesktopClient = new DesktopServiceClientWrapper(desktopClient, serviceProvider.GetRequiredService<ILogger<DesktopServiceClientWrapper>>());
	}

	public IDesktopClientMethods DesktopClient { get; }
}