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

	protected override Task OnReloadAsync(CancellationToken cancellationToken)
	{
		BrightnessItems = new ObservableCollection<BrightnessItem>(new []
		{
			new BrightnessItem(){Name = "Test1", Value = 20},
			new BrightnessItem(){Name = "Test2", Value = 30},
		});

		BrightnessItems[0].UpdateCommand = new RelayCommand<BrightnessItem>(item => SaveBrightness(BrightnessItems[0]));
		BrightnessItems[1].UpdateCommand = new RelayCommand<BrightnessItem>(item => SaveBrightness(BrightnessItems[1]));

		return Task.CompletedTask;
	}

	[RelayCommand]
	public Task SaveBrightness(BrightnessItem brightness)
	{
		return _toast.Make($"{brightness.Name} {string.Format(Translations.Monitors_Brightness_0, brightness.Value)}").Show();
	}
}

public partial class BrightnessItem : ObservableObject
{
	[ObservableProperty]
	private string _name;

	[ObservableProperty]
	private int _value;

	[ObservableProperty]
	private ICommand _updateCommand;
}