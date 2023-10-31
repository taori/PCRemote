namespace Amusoft.PCR.Domain.Services;

public interface INavigation
{
	public Task GoToAsync(string path, Dictionary<string, object>? parameters = null);
}