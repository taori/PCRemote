using System.Net;
using Amusoft.PCR.AM.UI.Interfaces;

namespace Amusoft.PCR.Int.UI.ProjectDependencies;

internal class DesktopIntegrationServiceFactory : IDesktopIntegrationServiceFactory
{
	private readonly IServiceProvider _serviceProvider;
	private readonly IGrpcChannelFactory _grpcChannelFactory;

	public DesktopIntegrationServiceFactory(IServiceProvider serviceProvider, IGrpcChannelFactory grpcChannelFactory)
	{
		_serviceProvider = serviceProvider;
		_grpcChannelFactory = grpcChannelFactory;
	}

	public IIpcIntegrationService Create(string protocol, IPEndPoint endPoint)
	{
		AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
		var channel = _grpcChannelFactory.Create(protocol, endPoint);
		return new IpcIntegrationService(channel, _serviceProvider);
	}
}