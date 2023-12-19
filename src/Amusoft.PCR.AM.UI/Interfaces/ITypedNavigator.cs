using System.Net;
using Amusoft.PCR.AM.UI.ViewModels;
using Amusoft.PCR.Domain.UI.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface ITypedNavigator
{
	Task PopAsync();
	Task OpenHost(IPEndPoint endPoint, string title, string protocol);
	Task OpenHostOverview();
	Task OpenSettings();
	Task OpenHostAccounts();
	Task OpenAudio();
	Task OpenSystemState();
	Task OpenMonitors();
	Task OpenInputControl();
	Task OpenPrograms();
	Task OpenLogs();
	Task OpenLogSettings();
	Task OpenDebug();
	Task OpenEndpointAccountPermissions(EndpointAccount account);
	Task OpenCommandButtonList(Action<CommandButtonListViewModel> configure, HostViewModel host);
	Task OpenMouseControl();
	Task ScopedNavigationAsync(Action<IServiceCollection> scopeConfiguration, Func<ITypedNavigator, Task> navigate);
}