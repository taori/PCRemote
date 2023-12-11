using Amusoft.PCR.Domain.UI.ValueTypes;

namespace Amusoft.PCR.Domain.UI.Entities;

public class LogEntry
{
	public DateTime Time { get; set; }

	public LogEntryType LogLevel { get; set; }

	public string Message { get; set; }

	public string Logger { get; set; }

	public string StackTrace { get; set; }

	public string CallSite { get; set; }
}