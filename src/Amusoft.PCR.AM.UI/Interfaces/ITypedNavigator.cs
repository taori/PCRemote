using System.Net;
using Amusoft.PCR.AM.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface ITypedNavigator
{
	Task PopAsync();
	Task OpenHost(IPEndPoint endPoint, string title);
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