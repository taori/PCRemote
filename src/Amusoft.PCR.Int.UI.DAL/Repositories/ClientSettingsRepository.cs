using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Domain.UI.Entities;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.UI.DAL.Repositories;

internal class ClientSettingsRepository : IClientSettingsRepository
{
	private readonly IFileStorage _fileStorage;
	private readonly ILogger<ClientSettingsRepository> _logger;
	private string _path = "clientsettings.json";

	public ClientSettingsRepository(IFileStorage fileStorage, ILogger<ClientSettingsRepository> logger)
	{
		_fileStorage = fileStorage;
		_logger = logger;
	}

	public Task<Settings> GetAsync(CancellationToken cancellationToken)
	{
		if (!_fileStorage.PathExists(_path))
			return Task.FromResult(new Settings());

		return _fileStorage.ReadJsonAsync<Settings>(_path, cancellationToken)!;
	}

	public async Task<bool> UpdateAsync(Action<Settings> update, CancellationToken cancellationToken)
	{
		var settings = await GetAsync(cancellationToken).ConfigureAwait(false);
		update(settings);
		return await SaveAsync(settings, cancellationToken).ConfigureAwait(false);
	}

	public async Task<bool> SaveAsync(Settings value, CancellationToken cancellationToken)
	{
		try
		{
			await _fileStorage.WriteJsonAsync(_path, value, cancellationToken);
			return true;
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error occured while saving");
			return false;
		}
	}
}