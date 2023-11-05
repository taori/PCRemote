using System.Net;
using Amusoft.PCR.Application.Features.DesktopIntegration;
using Grpc.Net.Client;

namespace Amusoft.PCR.App.UI.Implementations;

public class DesktopIntegrationServiceFactory : IDesktopIntegrationServiceFactory
{
	private readonly IServiceProvider _serviceProvider;

	public DesktopIntegrationServiceFactory(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public IDesktopIntegrationService Create(string protocol, IPEndPoint endPoint)
	{
		var target = $"{protocol}://{endPoint}";
		var channel = GrpcChannel.ForAddress(target);
		return new DesktopIntegrationService(channel, _serviceProvider);
	}
}