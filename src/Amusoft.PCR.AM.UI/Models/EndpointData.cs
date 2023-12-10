using System.Net;
using Amusoft.PCR.AM.UI.Interfaces;

namespace Amusoft.PCR.AM.UI.Models;

public class EndpointData : IHostCredentialProvider
{
	public EndpointData(IPEndPoint address, string title, string protocol)
	{
		Address = address;
		Title = title;
		Protocol = protocol;
	}

	public IPEndPoint Address { get; }
	public string Title { get; }
	public string Protocol { get; }
}