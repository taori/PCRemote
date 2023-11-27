using Amusoft.PCR.AM.UI.Extensions;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using CommunityToolkit.Mvvm.Input;
using Translations = Amusoft.PCR.AM.Shared.Resources.Translations;

namespace Amusoft.PCR.AM.UI.ViewModels;

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
	private Task Hibernate()
	{
		// hibernation does not return a result before entering hibernation so it cannot be awaited
		_ = _host.DesktopIntegrationClient.Desktop(d => d.Hibernate());
		return Task.CompletedTask;
	}
}