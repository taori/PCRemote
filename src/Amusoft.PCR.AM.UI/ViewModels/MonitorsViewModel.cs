using System.Collections.ObjectModel;
using System.Windows.Input;
using Amusoft.PCR.AM.UI.Extensions;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Translations = Amusoft.PCR.AM.Shared.Resources.Translations;

namespace Amusoft.PCR.AM.UI.ViewModels;

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

		var vmItems = monitors.Value.Select(d => new BrightnessItem(d.Id, d.Name, d.Current, d.Min, d.Max, null)).ToArray();

		foreach (var item in vmItems)
		{
			item.UpdateCommand = new RelayCommand(async() => await SaveBrightness(item));
		}
		
		BrightnessItems = new ObservableCollection<BrightnessItem>(vmItems);
	}

	[RelayCommand]
	public async Task SaveBrightness(BrightnessItem brightness)
	{
		if (_host.DesktopIntegrationClient?.DesktopClient == null)
			return;

		await _host.DesktopIntegrationClient.DesktopClient.SetMonitorBrightness(brightness.Id, brightness.Value);
		await _toast.Make(string.Format(AM.Shared.Resources.Translations.Monitors_Brightness_0, (object?)brightness.Value)).Show();
	}
}

public partial class BrightnessItem : ObservableObject
{
	public BrightnessItem(string id, string name, int value, int min, int max, ICommand? updateCommand)
	{
		_id = id;
		_name = name;
		_value = value;
		_min = min;
		_max = max;
		_updateCommand = updateCommand;
	}

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
	private ICommand? _updateCommand;
}