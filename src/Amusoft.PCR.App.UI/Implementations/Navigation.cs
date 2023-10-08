﻿using INavigation = Amusoft.PCR.Domain.Services.INavigation;

namespace Amusoft.PCR.App.UI.Implementations;

public class Navigation : Domain.Services.INavigation
{
	public Task GoToAsync(string path)
	{
		return Shell.Current.GoToAsync(path);
	}
}