#nullable enable

namespace Amusoft.PCR.App.UI.Implementations;

public class Navigation : AM.UI.Interfaces.INavigation
{
	public Task GoToAsync(string path, Dictionary<string, object>? parameters = null)
	{
		return parameters == null 
			? Shell.Current.GoToAsync(path) 
			: Shell.Current.GoToAsync(path, parameters);
	}
}