﻿using System.Net;
using Amusoft.PCR.Application.Features.DesktopIntegration;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Int.IPC;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.App.UI.Implementations;

public class DesktopIntegrationService : IDesktopIntegrationService
{
	private readonly VoiceCommandService.VoiceCommandServiceClient _voiceCommandClient;

	private readonly Int.IPC.DesktopIntegrationService.DesktopIntegrationServiceClient _desktopClient;

	public DesktopIntegrationService(GrpcChannel channel, IServiceProvider serviceProvider)
	{
		_voiceCommandClient = new VoiceCommandService.VoiceCommandServiceClient(channel);
		_desktopClient = new Int.IPC.DesktopIntegrationService.DesktopIntegrationServiceClient(channel);
		DesktopClient = new DesktopServiceClientWrapper(_desktopClient, serviceProvider.GetRequiredService<ILogger<DesktopServiceClientWrapper>>());
	}

	public IDesktopClientMethods DesktopClient { get; }
}