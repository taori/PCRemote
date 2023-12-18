using System.Collections.Concurrent;
using System.Net;
using Amusoft.PCR.AM.UI.Interfaces;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.UI.ProjectDependencies;

public class GrpcChannelFactory : IGrpcChannelFactory
{
	private readonly ILoggerFactory? _loggerFactory;
	private readonly IBearerTokenManager _tokenManager;

	private static readonly ConcurrentDictionary<IPEndPoint, HttpClient> ClientByEndpoint = new();

	public GrpcChannelFactory(ILoggerFactory? loggerFactory, IBearerTokenManager tokenManager)
	{
		_loggerFactory = loggerFactory;
		_tokenManager = tokenManager;
	}

	public GrpcChannel Create(string protocol, IPEndPoint endPoint, Func<Task<string>>? accessTokenProvider = null)
	{
		var client = ClientByEndpoint.GetOrAdd(endPoint, ClientFactory);
		var credentials = CallCredentials.FromInterceptor(async (context, metadata) =>
		{
			var token = accessTokenProvider is null
				? await _tokenManager.GetAccessTokenAsync(endPoint, context.CancellationToken, protocol).ConfigureAwait(false)
				: await accessTokenProvider();
			
			if (!string.IsNullOrEmpty(token))
				metadata.Add("Authorization", $"Bearer {token}");
		});
		var security = protocol == "http" ? ChannelCredentials.Insecure : ChannelCredentials.SecureSsl;
		var target = $"{protocol}://{endPoint}";
		var channel = GrpcChannel.ForAddress(target, new GrpcChannelOptions()
		{
			UnsafeUseInsecureChannelCallCredentials = true,
			Credentials = ChannelCredentials.Create(security, credentials),
			HttpClient = client,
			// LoggerFactory = _loggerFactory
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