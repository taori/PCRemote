using Amusoft.PCR.AM.UI.Repositories;
using OneOf;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IHostRepository
{
	Task<int[]> GetHostPortsAsync();
	Task RemoveAsync(int value);
	Task<OneOf<Success, AlreadyExists>> AddAsync(int value);
}