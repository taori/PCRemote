using Amusoft.PCR.AM.Service.Interfaces;
using Amusoft.PCR.Domain.Service.Entities;
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

	public ICollection<ServerConnection> Addresses => GetPorts(_addressesFeature.Addresses);

	private ICollection<ServerConnection> GetPorts(ICollection<string> addresses)
	{
		_logger.LogDebug(
			"Found {Number} addesses ({List}). Ports will be filtered by \"https:\"",
			_addressesFeature.Addresses.Count,
			string.Join(",", _addressesFeature.Addresses));

		var sub = addresses
			.Where(d => d.StartsWith("https:"))
			.Select(d => new ServerConnection(int.Parse(d.Split(":")[^1]), d.Split(":")[0]));
		return new List<ServerConnection>(sub);
	}
}