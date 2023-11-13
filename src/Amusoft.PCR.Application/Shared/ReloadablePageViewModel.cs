using Amusoft.PCR.Application.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.Application.Shared;

public abstract partial class ReloadablePageViewModel : PageViewModel
{
	private CancellationTokenSource? _reloadCts;
	[RelayCommand(AllowConcurrentExecutions = false)]
	protected async Task ReloadAsync()
	{
		_reloadCts?.Cancel(false);
		_reloadCts?.Dispose();
		_reloadCts = new();

		IsReloading = true;
		await OnReloadAsync(_reloadCts.Token);
		IsReloading = false;
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