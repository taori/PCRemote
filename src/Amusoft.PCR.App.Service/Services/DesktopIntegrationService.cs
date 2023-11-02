using Amusoft.PCR.Int.IPC;
using Grpc.Core;

namespace Amusoft.PCR.App.Service.Services;

public class DesktopIntegrationService : Int.IPC.DesktopIntegrationService.DesktopIntegrationServiceBase
{
	public override Task<AbortShutdownReply> AbortShutDown(AbortShutdownRequest request, ServerCallContext context)
	{
		return base.AbortShutDown(request, context);
	}

	public override Task<ShutdownDelayedReply> ShutDownDelayed(ShutdownDelayedRequest request, ServerCallContext context)
	{
		return base.ShutDownDelayed(request, context);
	}
}