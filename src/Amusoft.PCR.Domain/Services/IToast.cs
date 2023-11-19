namespace Amusoft.PCR.Domain.Services;

public enum Position
{
	Bottom, Left, Right, Top, Center
}

public interface IToast
{
	public IToastable Make(string text, bool shortDuration = true, double textSize = 14);
}

public interface IToastable : IDisposable
{
	Task Show();
	Task Dismiss();
	IToastable SetDuration(TimeSpan value);
	IToastable SetText(string value);
	IToastable SetPosition(Position value, int xOffset = 0, int yOffset = 0);
	IToastable SetTextSize(double textSize);
}