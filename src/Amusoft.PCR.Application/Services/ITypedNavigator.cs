using Amusoft.PCR.Application.UI.VM;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.Application.Services;

public interface ITypedNavigator
{
	Task PopAsync();
	Task OpenHost(Action<HostViewModel> configureModel);
	Task OpenHostOverview();
	Task OpenSettings();
	Task OpenAudio();
	Task OpenSystemState();
	Task OpenMonitors();
	Task OpenInputControl();
	Task OpenPrograms();
	Task OpenLogs();
	Task OpenDebug();
	Task OpenCommandButtonList(Action<CommandButtonListViewModel> configure, HostViewModel host);
	Task OpenMouseControl();
	Task ScopedNavigationAsync(Action<IServiceCollection> scopeConfiguration, Func<ITypedNavigator, Task> navigate);
}