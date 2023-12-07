using Microsoft.EntityFrameworkCore;

namespace Amusoft.PCR.Int.Service.Authorization;


public interface IPermissionService
{
	Task<List<Permission>> GetByKindAsync(PermissionKind permissionKind);
	Task<bool> AddPermissions(List<Permission> permissions);
	Task<bool> RevokeBySubjectAndKindAsync(PermissionKind permissionKind, string subjectId);
}

public class PermissionService : IPermissionService
{
	private readonly ApplicationDbContext _dbContext;

	public PermissionService(ApplicationDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<List<Permission>> GetByKindAsync(PermissionKind permissionKind)
	{
		return await _dbContext.Permissions.Where(d => d.PermissionType == permissionKind)
			.AsNoTracking()
			.ToListAsync();
	}
		
	public async Task<bool> AddPermissions(List<Permission> permissions)
	{
		foreach (var permission in permissions)
		{
			_dbContext.Permissions.Add(permission);
		}

		return await _dbContext.SaveChangesAsync() == permissions.Count;
	}

	public async Task<bool> RevokeBySubjectAndKindAsync(PermissionKind permissionKind, string subjectId)
	{
		_dbContext.Permissions.RemoveRange(_dbContext.Permissions
			.Where(d => d.PermissionType == permissionKind && d.SubjectId == subjectId));

		return await _dbContext.SaveChangesAsync() > 0;
	}
}