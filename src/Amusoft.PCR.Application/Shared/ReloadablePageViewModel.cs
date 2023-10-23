using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.Application.Shared;

public abstract partial class ReloadablePageViewModel : PageViewModel
{
	[RelayCommand(AllowConcurrentExecutions = false)]
	protected async Task ReloadAsync()
	{
		IsReloading = true;
		await OnReloadAsync();
		IsReloading = false;
	}

	protected abstract Task OnReloadAsync();

	[ObservableProperty]
	private bool _isReloading;
}