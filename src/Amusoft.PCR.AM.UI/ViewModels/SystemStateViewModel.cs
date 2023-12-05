using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using CommunityToolkit.Mvvm.Input;
using Translations = Amusoft.PCR.AM.Shared.Resources.Translations;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class SystemStateViewModel : PageViewModel
{
	private readonly HostViewModel _host;
	private readonly IDelayedSystemStateWorker _delayedSystemStateWorker;
	private readonly IUserInterfaceService _userInterfaceService;

	public SystemStateViewModel(ITypedNavigator navigator, HostViewModel host, IDelayedSystemStateWorker delayedSystemStateWorker, IUserInterfaceService userInterfaceService) : base(navigator)
	{
		_host = host;
		_delayedSystemStateWorker = delayedSystemStateWorker;
		_userInterfaceService = userInterfaceService;
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.Nav_SystemState;
	}

	[RelayCommand]
	private async Task Shutdown()
	{
		if (await _userInterfaceService.GetTimeFromPickerAsync(Translations.SystemState_PickDelay, TimeSpan.FromMinutes(1)) is { } delay)
			await _delayedSystemStateWorker.ShutdownAtAsync(DateTimeOffset.Now.Add(delay), false);
	}

	[RelayCommand]
	private async Task Restart()
	{
		if (await _userInterfaceService.GetTimeFromPickerAsync(Translations.SystemState_PickDelay, TimeSpan.FromMinutes(1)) is { } delay)
			await _delayedSystemStateWorker.RestartAtAsync(DateTimeOffset.Now.Add(delay), false);
	}

	[RelayCommand]
	private Task Abort()
	{
		_delayedSystemStateWorker.Clear();
		return _host.IpcClient.DesktopClient.AbortShutdown();
	}

	[RelayCommand]
	private Task Lock() =>
		_host.IpcClient.DesktopClient.LockWorkStation();

	[RelayCommand]
	private async Task Hibernate()
	{
		// hibernation does not return a result before entering hibernation so it cannot be awaited
		if (await _userInterfaceService.GetTimeFromPickerAsync(Translations.SystemState_PickDelay, TimeSpan.FromMinutes(1)) is { } delay)
			_ = _delayedSystemStateWorker.HibernateAtAsync(DateTimeOffset.Now.Add(delay));
	}
}