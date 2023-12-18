using System.Net;
using Grpc.Net.Client;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IGrpcChannelFactory
{
	/// <summary>
	/// Factory method which allows creation of a Grpc Channel to a given endpoint
	/// </summary>
	/// <param name="protocol">http/https</param>
	/// <param name="endPoint">e.g. 1.1.1.1:80</param>
	/// <param name="accessTokenProvider">if null, defaults to using automatic token provisioning</param>
	/// <returns></returns>
	GrpcChannel Create(string protocol, IPEndPoint endPoint, Func<Task<string>>? accessTokenProvider = default);
}