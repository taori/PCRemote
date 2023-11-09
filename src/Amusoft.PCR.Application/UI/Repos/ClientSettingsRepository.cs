using Amusoft.PCR.Domain.ClientSettings;
using Amusoft.PCR.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Application.UI.Repos;

public class ClientSettingsRepository
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

	public async Task<bool> SaveAsync(CancellationToken cancellationToken, Settings value)
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