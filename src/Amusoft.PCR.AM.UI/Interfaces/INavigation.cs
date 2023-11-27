namespace Amusoft.PCR.AM.UI.Interfaces;

public interface INavigation
{
	public Task GoToAsync(string path, Dictionary<string, object>? parameters = null);
}