using Amusoft.PCR.AM.Service.Interfaces;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace Amusoft.PCR.App.Service.Services;

public class ConnectedServerPorts : IConnectedServerPorts
{
	private readonly ILogger<ConnectedServerPorts> _logger;
	private readonly IServerAddressesFeature _addressesFeature;

	public ConnectedServerPorts(IServer server, ILogger<ConnectedServerPorts> logger)
	{
		_logger = logger;
		_addressesFeature = server.Features.Get<IServerAddressesFeature>()!;
	}

	public ICollection<int> Addresses => GetPorts(_addressesFeature.Addresses);

	private ICollection<int> GetPorts(ICollection<string> addresses)
	{
		_logger.LogDebug("Found {Number} addesses ({List}). Ports will be filtered by \"http:\"",
			_addressesFeature.Addresses.Count,
			string.Join(",", _addressesFeature.Addresses));

		var sub = addresses
			.Where(d => d.StartsWith("http:"))
			.Select(d => int.Parse(d.Split(":")[^1]));
		return new List<int>(sub);
	}
}