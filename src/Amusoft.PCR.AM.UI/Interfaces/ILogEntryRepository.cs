using Amusoft.PCR.Domain.UI.Entities;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface ILogEntryRepository
{
	Task<List<LogEntry>> GetLogsSinceAsync(DateTime since, CancellationToken cancellationToken);
}