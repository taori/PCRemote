using System.Net;
using Grpc.Net.Client;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IGrpcChannelFactory
{
	GrpcChannel Create(string protocol, IPEndPoint endPoint);
}