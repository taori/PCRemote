using Amusoft.PCR.Domain.Services;

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

		return await _fileStorage.ReadJsonAsync<int[]>(_hostsPath, CancellationToken.None).ConfigureAwait(false);
	}

	public Task SaveHostPortsAsync(int[] ports)
	{
		return _fileStorage.WriteJsonAsync(_hostsPath, ports, CancellationToken.None);
	}
}