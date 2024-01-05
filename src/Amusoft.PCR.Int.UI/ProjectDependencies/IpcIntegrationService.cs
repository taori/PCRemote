using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Domain.Shared.Interfaces;
using Amusoft.PCR.Int.IPC;
using Amusoft.PCR.Int.IPC.Integration;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.UI.ProjectDependencies;

public class IpcIntegrationService : IIpcIntegrationService
{
	public IpcIntegrationService(GrpcChannel channel, IServiceProvider serviceProvider)
	{
		_voiceCommandClient = new VoiceCommandService.VoiceCommandServiceClient(channel);
		var desktopClient = new DesktopIntegrationService.DesktopIntegrationServiceClient(channel);
		var userManagementClient = new UserManagement.UserManagementClient(channel);
		var acquisionService = new PermissionAcquisionService(userManagementClient, serviceProvider.GetRequiredService<ILogger<PermissionAcquisionService>>());
		DesktopClient = new DesktopServiceClientWrapper(desktopClient, serviceProvider.GetRequiredService<ILogger<DesktopServiceClientWrapper>>(), acquisionService);
		IdentityExtendedClient = new UserExtendedClientWrapper(userManagementClient, serviceProvider.GetRequiredService<ILogger<UserExtendedClientWrapper>>());
	}

	private readonly VoiceCommandService.VoiceCommandServiceClient _voiceCommandClient;

	public IDesktopClientMethods DesktopClient { get; }

	public IIdentityExtendedClient IdentityExtendedClient { get; }
}