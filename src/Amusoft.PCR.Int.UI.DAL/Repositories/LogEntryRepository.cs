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

	public Task<List<LogEntry>> GetLogsAsync(int? page, LogSettings settings, CancellationToken cancellationToken)
	{
		if (page is null)
		{
			return GetSubsetWithSettings(settings)
				.OrderByDescending(d => d.Time)
				.ToListAsync(cancellationToken);
		}
		else
		{
			return GetSubsetWithSettings(settings)
				.Skip(page.Value * settings.EntriesPerPage)
				.Take(settings.EntriesPerPage)
				.OrderByDescending(d => d.Time)
				.ToListAsync(cancellationToken);
		}
	}

	public async Task<bool> DeleteAllAsync(CancellationToken cancellationToken)
	{
		return await _dbContext.LogEntries
			.ExecuteDeleteAsync(cancellationToken).ConfigureAwait(false) > 0;
	}

	public Task<int> GetCountAsync(LogSettings? settings, CancellationToken cancellationToken)
	{
		if (settings is not null)
		{
			return GetSubsetWithSettings(settings)
				.CountAsync(cancellationToken);
		}

		return _dbContext.LogEntries
			.CountAsync(cancellationToken);
	}

	private IQueryable<LogEntry> GetSubsetWithSettings(LogSettings settings)
	{
		var since = DateTime.Now.Add(settings.ShowRecent * -1);
		return _dbContext.LogEntries
			.Where(d => d.Time > since && d.LogLevel >= settings.LogLevel);
	}
}