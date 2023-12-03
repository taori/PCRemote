namespace Amusoft.PCR.AM.UI.Interfaces;

public interface INavigationCallbacks
{
	public Task OnNavigatedToAsync() => Task.CompletedTask;
	public Task OnNavigatedAwayAsync() => Task.CompletedTask;
}