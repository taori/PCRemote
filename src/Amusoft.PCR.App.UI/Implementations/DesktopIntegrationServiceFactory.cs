using System.Collections.Concurrent;
using System.Net;
using Amusoft.PCR.Application.Features.DesktopIntegration;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Grpc.Net.ClientFactory;
using Microsoft.Extensions.Logging;

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

public class GrpcChannelFactory
{
	private readonly ILoggerFactory _loggerFactory;

	private static readonly ConcurrentDictionary<IPEndPoint, HttpClient> ClientByEndpoint = new();

	public GrpcChannelFactory(ILoggerFactory loggerFactory)
	{
		_loggerFactory = loggerFactory;
	}

	public GrpcChannel Create(string protocol, IPEndPoint endPoint)
	{
		var client = ClientByEndpoint.GetOrAdd(endPoint, ClientFactory);
		var target = $"{protocol}://{endPoint}";
		var channel = GrpcChannel.ForAddress(target, new GrpcChannelOptions()
		{
			HttpClient = client,
			LoggerFactory = _loggerFactory
		});

		return channel;
	}

	private HttpClient ClientFactory(IPEndPoint endPoint)
	{
		var innerHandler = new HttpClientHandler() {};
		var outerHandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, innerHandler)
		{
			HttpVersion = HttpVersion.Version11
		};
		return new HttpClient(outerHandler, true)
		{
			DefaultRequestVersion = HttpVersion.Version11,
			DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact
		};
	}
}