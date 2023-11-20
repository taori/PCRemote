using Amusoft.PCR.Application.Resources;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.Shared;
using CommunityToolkit.Mvvm.Input;
using System.Runtime.InteropServices.JavaScript;
using Amusoft.PCR.Application.Extensions;

namespace Amusoft.PCR.Application.UI.VM;

public partial class MonitorsViewModel : PageViewModel
{
	private readonly HostViewModel _host;

	public MonitorsViewModel(ITypedNavigator navigator, HostViewModel host) : base(navigator)
	{
		_host = host;
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.Nav_Monitors;
	}

	[RelayCommand]
	private Task MonitorOff() =>
		_host.DesktopIntegrationClient.Desktop(d => d.MonitorOff());

	[RelayCommand]
	private Task MonitorOn() =>
		_host.DesktopIntegrationClient.Desktop(d => d.MonitorOn());
}