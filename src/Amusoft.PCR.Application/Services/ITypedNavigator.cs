﻿using Amusoft.PCR.Application.UI.VM;

namespace Amusoft.PCR.Application.Services;

public interface ITypedNavigator
{
	Task PopAsync();
	Task OpenHost(Action<HostViewModel> configureModel);
	Task OpenHostOverview();
	Task OpenSettings();
}