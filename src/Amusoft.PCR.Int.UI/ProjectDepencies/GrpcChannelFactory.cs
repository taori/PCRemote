#region

using System.Collections.Concurrent;
using System.Net;
using Amusoft.PCR.AM.UI.Interfaces;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.Extensions.Logging;

#endregion

namespace Amusoft.PCR.Int.UI.ProjectDepencies;

public class GrpcChannelFactory : IGrpcChannelFactory
{
	private readonly ILoggerFactory? _loggerFactory;
	private readonly IBearerTokenProvider _tokenProvider;

	private static readonly ConcurrentDictionary<IPEndPoint, HttpClient> ClientByEndpoint = new();

	public GrpcChannelFactory(ILoggerFactory? loggerFactory, IBearerTokenProvider tokenProvider)
	{
		_loggerFactory = loggerFactory;
		_tokenProvider = tokenProvider;
	}

	public GrpcChannel Create(string protocol, IPEndPoint endPoint)
	{
		var client = ClientByEndpoint.GetOrAdd(endPoint, ClientFactory);
		var credentials = CallCredentials.FromInterceptor(async (context, metadata) =>
		{
			var token = await _tokenProvider.GetAccessTokenAsync(endPoint, context.CancellationToken).ConfigureAwait(false);
			if (!string.IsNullOrEmpty(token))
				metadata.Add("Authorization", $"Bearer {token}");
		});
		var target = $"{protocol}://{endPoint}";
		var channel = GrpcChannel.ForAddress(target, new GrpcChannelOptions()
		{
			UnsafeUseInsecureChannelCallCredentials = true,
			Credentials = ChannelCredentials.Create(ChannelCredentials.Insecure, credentials),
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