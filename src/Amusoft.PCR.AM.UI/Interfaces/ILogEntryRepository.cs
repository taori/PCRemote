using Amusoft.PCR.Domain.UI.Entities;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface ILogEntryRepository
{
	Task<List<LogEntry>> GetLogsAsync(LogSettings settings, CancellationToken cancellationToken);
	Task<bool> DeleteAllAsync(CancellationToken cancellationToken);
	Task<int> GetCountAsync(CancellationToken cancellationToken);
}