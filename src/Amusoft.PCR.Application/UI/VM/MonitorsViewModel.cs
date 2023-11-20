using System.Collections.ObjectModel;
using Amusoft.PCR.Application.Resources;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.Shared;
using CommunityToolkit.Mvvm.Input;
using System.Runtime.InteropServices.JavaScript;
using System.Windows.Input;
using Amusoft.PCR.Application.Extensions;
using Amusoft.PCR.Domain.Services;
using Amusoft.PCR.Domain.VM;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Amusoft.PCR.Application.UI.VM;

public partial class MonitorsViewModel : ReloadablePageViewModel, INavigationCallbacks
{
	private readonly HostViewModel _host;
	private readonly IToast _toast;

	public MonitorsViewModel(ITypedNavigator navigator, HostViewModel host, IToast toast) : base(navigator)
	{
		_host = host;
		_toast = toast;
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

	[ObservableProperty]
	private ObservableCollection<BrightnessItem>? _brightnessItems;

	public Task OnNavigatedToAsync()
	{
		return ReloadAsync();
	}

	protected override async Task OnReloadAsync(CancellationToken cancellationToken)
	{
		if (_host.DesktopIntegrationClient?.DesktopClient is null)
			return;

		var monitors = await _host.DesktopIntegrationClient.DesktopClient.GetMonitorBrightness();
		if (!monitors.Success)
			return;

		var vmItems = monitors.Value.Select(d => new BrightnessItem()
		{
			Value = d.Current,
			Name = d.Name,
			Min = d.Min,
			Max = d.Max,
			Id = d.Id,
		}).ToArray();

		foreach (var item in vmItems)
		{
			item.UpdateCommand = new RelayCommand(() => SaveBrightness(item));
		}
		
		BrightnessItems = new ObservableCollection<BrightnessItem>(vmItems);
	}

	[RelayCommand]
	public async Task SaveBrightness(BrightnessItem brightness)
	{
		if (_host.DesktopIntegrationClient?.DesktopClient == null)
			return;

		await _host.DesktopIntegrationClient.DesktopClient.SetMonitorBrightness(brightness.Id, brightness.Value);
		await _toast.Make(string.Format(Translations.Monitors_Brightness_0, brightness.Value)).Show();
	}
}

public partial class BrightnessItem : ObservableObject
{
	[ObservableProperty]
	private string _id;

	[ObservableProperty]
	private string _name;

	[ObservableProperty]
	private int _value;

	[ObservableProperty]
	private int _min;

	[ObservableProperty]
	private int _max;

	[ObservableProperty]
	private ICommand _updateCommand;
}