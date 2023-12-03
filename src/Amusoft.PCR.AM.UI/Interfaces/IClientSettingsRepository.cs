using Amusoft.PCR.Domain.UI.Entities;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IClientSettingsRepository
{
	Task<Settings> GetAsync(CancellationToken cancellationToken);
	Task<bool> UpdateAsync(Action<Settings> update, CancellationToken cancellationToken);
	Task<bool> SaveAsync(Settings value, CancellationToken cancellationToken);
}