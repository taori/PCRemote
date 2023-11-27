namespace Amusoft.PCR.Domain.VM;

public interface INavigationCallbacks
{
	public Task OnNavigatedToAsync() => Task.CompletedTask;
	public Task OnNavigatedAwayAsync() => Task.CompletedTask;
}