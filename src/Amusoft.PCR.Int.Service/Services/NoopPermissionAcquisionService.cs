using Amusoft.PCR.Domain.Shared.Interfaces;

namespace Amusoft.PCR.Int.Service.Services;

internal class NoopPermissionAcquisionService : IPermissionAcquisionService
{
	public Task<bool> AcquireRolesAsync(string[] roles)
	{
		return Task.FromResult(false);
	}
}