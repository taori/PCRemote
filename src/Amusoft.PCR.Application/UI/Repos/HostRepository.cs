using Amusoft.PCR.Domain.Services;
using OneOf;

namespace Amusoft.PCR.Application.UI.Repos;

public class HostRepository
{
	private readonly IFileStorage _fileStorage;
	private string _hostsPath = "hosts.json";

	public HostRepository(IFileStorage fileStorage)
	{
		_fileStorage = fileStorage;
	}

	public async Task<int[]> GetHostPortsAsync()
	{
		if (!_fileStorage.PathExists(_hostsPath))
			return Array.Empty<int>();

		return await _fileStorage.ReadJsonAsync<int[]>(_hostsPath, CancellationToken.None).ConfigureAwait(false) ?? Array.Empty<int>();
	}

	private Task SaveHostPortsAsync(int[] ports)
	{
		return _fileStorage.WriteJsonAsync(_hostsPath, ports, CancellationToken.None);
	}

	public async Task RemoveAsync(int value)
	{
		var ports = await GetHostPortsAsync();
		if (!ports.Contains(value))
			return;

		await SaveHostPortsAsync(ports.Except(new[] { value }).ToArray());
	}

	public async Task<OneOf<Success, AlreadyExists>> AddAsync(int value)
	{
		var ports = await GetHostPortsAsync();
		if (ports.Contains(value))
			return new AlreadyExists();
		
		await SaveHostPortsAsync(ports.Concat(new[] {value}).ToArray());
		return new Success();
	}
}