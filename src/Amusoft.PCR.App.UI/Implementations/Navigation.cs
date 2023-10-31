#nullable enable

using INavigation = Amusoft.PCR.Domain.Services.INavigation;

namespace Amusoft.PCR.App.UI.Implementations;

public class Navigation : INavigation
{
	public Task GoToAsync(string path, Dictionary<string, object>? parameters = null)
	{
		return parameters == null 
			? Shell.Current.GoToAsync(path) 
			: Shell.Current.GoToAsync(path, parameters);
	}
}