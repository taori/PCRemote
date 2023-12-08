using Amusoft.PCR.Int.Service.Authorization;
using Amusoft.PCR.Int.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Amusoft.PCR.Int.Service.Services;

public class MigrationTask(ApplicationDbContext dbContext) : IStartupTask
{
	public int Priority => 100;
	
	public async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await dbContext.Database.MigrateAsync(stoppingToken);
	}
}