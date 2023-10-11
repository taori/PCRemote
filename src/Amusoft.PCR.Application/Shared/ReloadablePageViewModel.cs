using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.Application.Shared;

public abstract partial class ReloadablePageViewModel : PageViewModel
{
	[RelayCommand(AllowConcurrentExecutions = false)]
	protected virtual Task OnReloadAsync()
	{
		return Task.CompletedTask;
	}
}