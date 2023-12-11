using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Domain.UI.Entities;
using Amusoft.PCR.Int.UI.DAL.Database;
using Microsoft.EntityFrameworkCore;

namespace Amusoft.PCR.Int.UI.DAL.Repositories;

internal class LogEntryRepository : ILogEntryRepository
{
	private readonly UiDbContext _dbContext;

	public LogEntryRepository(UiDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public Task<List<LogEntry>> GetLogsSinceAsync(DateTime since, CancellationToken cancellationToken)
	{
		return _dbContext.LogEntries
			.Where(d => d.Time > since)
			.OrderByDescending(d => d.Time)
			.ToListAsync(cancellationToken);
	}
}