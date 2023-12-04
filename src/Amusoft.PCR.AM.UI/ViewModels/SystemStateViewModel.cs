using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using CommunityToolkit.Mvvm.Input;
using Translations = Amusoft.PCR.AM.Shared.Resources.Translations;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class SystemStateViewModel : PageViewModel
{
	private readonly HostViewModel _host;
	private readonly IDelayedSystemStateWorker _delayedSystemStateWorker;

	public SystemStateViewModel(ITypedNavigator navigator, HostViewModel host, IDelayedSystemStateWorker delayedSystemStateWorker) : base(navigator)
	{
		_host = host;
		_delayedSystemStateWorker = delayedSystemStateWorker;
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.Nav_SystemState;
	}

	[RelayCommand]
	private Task Shutdown()
	{
		return _delayedSystemStateWorker.ShutdownAtAsync(DateTimeOffset.Now.AddSeconds(30), false);
	}

	[RelayCommand]
	private Task Restart()
	{
		return _delayedSystemStateWorker.RestartAtAsync(DateTimeOffset.Now.AddSeconds(30), false);
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
	private Task Hibernate()
	{
		// hibernation does not return a result before entering hibernation so it cannot be awaited
		_ = _delayedSystemStateWorker.HibernateAtAsync(DateTimeOffset.Now.AddSeconds(30));
		return Task.CompletedTask;
	}
}