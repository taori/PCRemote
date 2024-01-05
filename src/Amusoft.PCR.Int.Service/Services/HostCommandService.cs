using Amusoft.PCR.Domain.Service.Entities;
using Amusoft.PCR.Int.Service.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Amusoft.PCR.Int.Service.Services;

public class HostCommandService : IHostCommandService
{
	private readonly ApplicationDbContext _context;

	public HostCommandService(ApplicationDbContext context)
	{
		_context = context;
	}

	public Task<List<HostCommand>> GetAllAsync()
	{
		return _context.HostCommands.ToListAsync();
	}

	public async Task<bool> CreateAsync(HostCommand item)
	{
		_context.HostCommands.Add(item);
		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<bool> DeleteAsync(HostCommand item)
	{
		_context.HostCommands.Remove(item);
		return await _context.SaveChangesAsync() > 0;
	}

	public async Task<HostCommand?> GetByIdAsync(string id)
	{
		return await _context.HostCommands.FindAsync(id);
	}

	public async Task<bool> UpdateAsync(HostCommand item)
	{
		// _context.Entry(item).State = EntityState.Modified;
		_context.HostCommands.Update(item);
		return await _context.SaveChangesAsync() > 0;
	}
}

public interface IHostCommandService
{
	Task<List<HostCommand>> GetAllAsync();
	Task<bool> CreateAsync(HostCommand item);
	Task<bool> DeleteAsync(HostCommand item);
	Task<HostCommand?> GetByIdAsync(string id);
	Task<bool> UpdateAsync(HostCommand item);
}