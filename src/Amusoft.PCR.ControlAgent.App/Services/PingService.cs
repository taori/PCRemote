using Amusoft.PCR.ControlAgent.Shared;
using Grpc.Core;

namespace Amusoft.PCR.ControlAgent.App.Services;

public class PingService : Ping.PingBase
{
	private readonly ILogger<PingService> _logger;
	public PingService(ILogger<PingService> logger)
	{
		_logger = logger;
	}

	public override Task<PongReply> Ping(PingRequest request, ServerCallContext context)
	{
		_logger.LogInformation("Received ping request from {Name}", request.Name);

		return Task.FromResult(new PongReply()
		{
			Message = "Hello " + request.Name
		});
	}
}