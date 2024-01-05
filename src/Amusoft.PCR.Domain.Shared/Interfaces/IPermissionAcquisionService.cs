namespace Amusoft.PCR.Domain.Shared.Interfaces;

public interface IPermissionAcquisionService
{
	Task<bool> AcquireRolesAsync(string[] roles);
}