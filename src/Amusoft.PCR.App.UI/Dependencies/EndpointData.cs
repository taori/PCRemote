using System.Net;
using Amusoft.PCR.AM.UI.Interfaces;

namespace Amusoft.PCR.App.UI.Dependencies;

internal class EndpointData : IHostCredentialProvider
{
	public EndpointData(IPEndPoint address, string title)
	{
		Address = address;
		Title = title;
	}

	public IPEndPoint Address { get; }
	public string Title { get; }
}