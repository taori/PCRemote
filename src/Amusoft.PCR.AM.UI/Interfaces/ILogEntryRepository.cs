using Amusoft.PCR.Domain.UI.Entities;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface ILogEntryRepository
{
	Task<List<LogEntry>> GetLogsAsync(int? page, LogSettings settings, CancellationToken cancellationToken);
	Task<bool> DeleteAllAsync(CancellationToken cancellationToken);
	Task<int> GetCountAsync(LogSettings? settings, CancellationToken cancellationToken);
}