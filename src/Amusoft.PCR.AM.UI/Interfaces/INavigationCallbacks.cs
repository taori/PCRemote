namespace Amusoft.PCR.AM.UI.Interfaces;

public interface INavigationCallbacks
{
	public Task OnNavigatedToAsync() => Task.CompletedTask;
	public Task OnNavigatingAsync(INavigatingContext context) => Task.CompletedTask;
}