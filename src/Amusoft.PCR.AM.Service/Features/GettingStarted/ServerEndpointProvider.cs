using System.Net.NetworkInformation;
using System.Net.Sockets;
using Amusoft.PCR.AM.Service.Interfaces;

namespace Amusoft.PCR.AM.Service.Features.GettingStarted;

public class ServerEndpointProvider
{
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly IConnectedServerPorts _connectedServerPorts;

	public ServerEndpointProvider(IHttpClientFactory httpClientFactory, IConnectedServerPorts connectedServerPorts)
	{
		_httpClientFactory = httpClientFactory;
		_connectedServerPorts = connectedServerPorts;
	}

	public record StatusModel(string Endpoint, bool Running);

	public async Task<List<StatusModel>> GetStatusModelsAsync()
	{
		var items = new List<StatusModel>();
		await foreach (var model in GetStatusModelsInternalAsync())
		{
			items.Add(model);
		}

		return items;
	}

	private async IAsyncEnumerable<StatusModel> GetStatusModelsInternalAsync()
	{
		foreach (var endpoint in GetEndpoints())
		{
			yield return new StatusModel(endpoint, await GetStatusAsync(endpoint));
		}
	}

	private IEnumerable<string> GetEndpoints()
	{
		foreach (var networkInterface in GetOperationalInterfaces())
		{
			foreach (var address in networkInterface.GetIPProperties().UnicastAddresses)
			{
				if(address.Address.AddressFamily != AddressFamily.InterNetwork)
					continue;

				foreach (var port in _connectedServerPorts.Addresses)
				{
					yield return $"http://{address.Address}:{port}";
				}
			}
		}
	}

	private static IEnumerable<NetworkInterface> GetOperationalInterfaces()
	{
		return NetworkInterface.GetAllNetworkInterfaces().Where((n) =>
			n.NetworkInterfaceType != NetworkInterfaceType.Loopback && n.OperationalStatus == OperationalStatus.Up);
	}

	private async Task<bool> GetStatusAsync(string address)
	{
		try
		{
			using var cts = new CancellationTokenSource(1000);
			var client = _httpClientFactory.CreateClient();
			var result = await client.GetAsync($"{address}/health",HttpCompletionOption.ResponseHeadersRead, cts.Token);
			return result.IsSuccessStatusCode;
		}
		catch (Exception)
		{
			return false;
		}
	}
	
}