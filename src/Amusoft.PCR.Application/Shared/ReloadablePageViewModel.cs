using Amusoft.PCR.Application.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NLog;

namespace Amusoft.PCR.Application.Shared;

public abstract partial class ReloadablePageViewModel : PageViewModel
{
	private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

	private CancellationTokenSource? _reloadCts;
	[RelayCommand(AllowConcurrentExecutions = false)]
	protected async Task ReloadAsync()
	{
		Log.Debug("Reloading");

		_reloadCts?.Cancel(false);
		_reloadCts?.Dispose();
		_reloadCts = new();
		try
		{
			IsReloading = true;
			await OnReloadAsync(_reloadCts.Token);
		}
		catch (OperationCanceledException)
		{
			// empty on purpose
		}
		finally
		{
			IsReloading = false;
		}
	}

	protected abstract Task OnReloadAsync(CancellationToken cancellationToken);

	[ObservableProperty]
	private bool _isReloading;

	public override void OnDispose(bool disposeManaged)
	{
		if (disposeManaged)
		{
			_reloadCts?.Dispose();
		}
		base.OnDispose(disposeManaged);
	}

	protected ReloadablePageViewModel(ITypedNavigator navigator) : base(navigator)
	{
	}
}