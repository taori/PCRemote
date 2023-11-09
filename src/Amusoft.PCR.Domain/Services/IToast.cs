namespace Amusoft.PCR.Domain.Services;

public enum Position
{
	Left, Right, Top, Bottom
}

public interface IToast
{
	public IToastable Make(string text, bool shortDuration = true, double textSize = 14);
}

public interface IToastable : IDisposable
{
	Task Show();
	Task Dismiss();
	IToastable SetText(string value);
	IToastable SetPosition(Position value, int xOffset = 0, int yOffset = 0);
}