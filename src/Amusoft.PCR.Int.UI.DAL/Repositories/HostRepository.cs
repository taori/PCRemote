using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.Repositories;
using OneOf;

namespace Amusoft.PCR.Int.UI.DAL.Repositories;

internal class HostRepository : IHostRepository
{
	private readonly IClientSettingsRepository _settingsRepository;

	public HostRepository(IClientSettingsRepository settingsRepository)
	{
		_settingsRepository = settingsRepository;
	}

	public async Task<int[]> GetHostPortsAsync()
	{
		var settings = await _settingsRepository.GetAsync(CancellationToken.None);
		return settings.Ports;
	}

	private async Task SaveHostPortsAsync(int[] ports)
	{
		var settings = await _settingsRepository.GetAsync(CancellationToken.None);
		settings.Ports = ports;
		await _settingsRepository.SaveAsync(settings, CancellationToken.None);
	}

	public async Task RemoveAsync(int value)
	{
		var settings = await _settingsRepository.GetAsync(CancellationToken.None);
		settings.Ports = settings.Ports.Except(new[] { value }).ToArray();
		await _settingsRepository.SaveAsync(settings, CancellationToken.None);
	}

	public async Task<OneOf<Success, AlreadyExists>> AddAsync(int value)
	{
		var settings = await _settingsRepository.GetAsync(CancellationToken.None);
		if (settings.Ports.Contains(value))
			return new AlreadyExists();
		
		await SaveHostPortsAsync(settings.Ports.Concat(new[] {value}).ToArray());
		return new Success();
	}
}