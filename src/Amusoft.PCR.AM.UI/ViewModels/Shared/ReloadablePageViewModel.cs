using Amusoft.PCR.AM.Shared.Utility;
using Amusoft.PCR.AM.UI.Interfaces;
using CommunityToolkit.Mvvm.Input;
using NLog;

namespace Amusoft.PCR.AM.UI.ViewModels.Shared;

public abstract partial class ReloadablePageViewModel : PageViewModel
{
	private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

	private readonly Cooldown _reloadCooldown = new(TimeSpan.FromMilliseconds(1000));

	public LoadState LoadState { get; } = new();

	[RelayCommand(AllowConcurrentExecutions = false)]
	protected async Task ReloadAsync()
	{
		if (!_reloadCooldown.TryClaim())
		{
			Log.Debug("Reload blocked because of cooldown");
			return;
		}
		
		Log.Debug("Reloading");

		var queue = LoadState.QueueLoading();
		try
		{
			Log.Trace("IsReloading = true");
			await OnReloadAsync(CancellationToken.None);
		}
		catch (OperationCanceledException)
		{
			// empty on purpose
		}
		finally
		{
			Log.Trace("IsReloading = false");
			queue.Dispose();
		}
	}

	protected abstract Task OnReloadAsync(CancellationToken cancellationToken);

	protected ReloadablePageViewModel(ITypedNavigator navigator) : base(navigator)
	{
	}
}