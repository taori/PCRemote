using System.Net;
using Amusoft.PCR.AM.UI.Interfaces;
using Grpc.Net.ClientFactory;

namespace Amusoft.PCR.App.UI.Implementations;

public class DesktopIntegrationServiceFactory : IDesktopIntegrationServiceFactory
{
	private readonly IServiceProvider _serviceProvider;
	private readonly GrpcChannelFactory _grpcChannelFactory;

	public DesktopIntegrationServiceFactory(IServiceProvider serviceProvider, GrpcChannelFactory grpcChannelFactory)
	{
		_serviceProvider = serviceProvider;
		_grpcChannelFactory = grpcChannelFactory;
	}

	public IDesktopIntegrationService Create(string protocol, IPEndPoint endPoint)
	{
		AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
		var channel = _grpcChannelFactory.Create(protocol, endPoint);
		return new DesktopIntegrationService(channel, _serviceProvider);
	}
}