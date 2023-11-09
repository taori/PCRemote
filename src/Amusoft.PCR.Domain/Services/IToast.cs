namespace Amusoft.PCR.Domain.Services;

public interface IToast
{
	public IToastable Make(string text, bool shortDuration = true, double textSize = 14);
}

public interface IToastable : IDisposable
{
	Task Show();
	Task Dismiss();
	// string Text { get; set; }
}