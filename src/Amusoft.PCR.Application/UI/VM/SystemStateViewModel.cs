using Amusoft.PCR.Application.Extensions;
using Amusoft.PCR.Application.Resources;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.Shared;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.Application.UI.VM;

public partial class SystemStateViewModel : PageViewModel
{
	private readonly HostViewModel _host;

	public SystemStateViewModel(ITypedNavigator navigator, HostViewModel host) : base(navigator)
	{
		_host = host;
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.Nav_SystemState;
	}

	[RelayCommand]
	private Task Shutdown() => 
		_host.DesktopIntegrationClient.Desktop(d => d.Shutdown(TimeSpan.FromMinutes(1), false));

	[RelayCommand]
	private Task Restart() =>
		_host.DesktopIntegrationClient.Desktop(d => d.Restart(TimeSpan.FromMinutes(1), false));

	[RelayCommand]
	private Task Abort() =>
		_host.DesktopIntegrationClient.Desktop(d => d.AbortShutdown());

	[RelayCommand]
	private Task Lock() =>
		_host.DesktopIntegrationClient.Desktop(d => d.LockWorkStation());

	[RelayCommand]
	private Task Hibernate() =>
		_host.DesktopIntegrationClient.Desktop(d => d.Hibernate());
}