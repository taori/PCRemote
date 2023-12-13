namespace Amusoft.PCR.Domain.UI.Entities;

public class LogSettings
{
	public TimeSpan ShowRecent { get; set; } = TimeSpan.FromDays(1);

	public bool DisplayFullLoggerName { get; set; } = true;

	public string DateFormat { get; set; } = "G";

	public int WidthDate { get; set; } = 140;

	public int WidthLogger { get; set; } = 140;

	public bool DisplayDate { get; set; } = true;

	public bool DisplayLogger { get; set; } = true;
}