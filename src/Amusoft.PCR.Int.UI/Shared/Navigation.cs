#nullable enable

namespace Amusoft.PCR.Int.UI.Shared;

public class Navigation : AM.UI.Interfaces.INavigation
{
	public Task GoToAsync(string path, Dictionary<string, object>? parameters = null)
	{
		return parameters == null 
			? Shell.Current.GoToAsync(path) 
			: Shell.Current.GoToAsync(path, parameters);
	}
}