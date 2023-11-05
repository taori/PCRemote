using Amusoft.PCR.Application.Features.DesktopIntegration;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace Amusoft.PCR.App.Service.Services;

public class ConnectedServerPorts : IConnectedServerPorts
{
	private readonly IServerAddressesFeature _addressesFeature;

	public ConnectedServerPorts(IServer server)
	{
		_addressesFeature = server.Features.Get<IServerAddressesFeature>()!;
	}

	public ICollection<int> Addresses => GetPorts(_addressesFeature.Addresses);

	private ICollection<int> GetPorts(ICollection<string> addresses)
	{
		var sub = addresses
			.Where(d => d.StartsWith("http:"))
			.Select(d => int.Parse(d.Split(":")[^1]));
		return new List<int>(sub);
	}
}