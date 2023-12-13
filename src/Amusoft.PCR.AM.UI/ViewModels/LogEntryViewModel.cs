using Amusoft.PCR.Domain.UI.Entities;
using Amusoft.PCR.Domain.UI.ValueTypes;

namespace Amusoft.PCR.AM.UI.ViewModels;

public class LogEntryViewModel
{
	private readonly LogEntry _item;
	private readonly LogSettings _settings;

	public LogEntryViewModel(LogEntry item, LogSettings settings)
	{
		_item = item;
		_settings = settings;
	}

	public string Time => _item.Time.ToString(_settings.DateFormat);
	public string Logger => _item.Logger;
	public string Message => _item.Message;
	public LogEntryType LogLevel => _item.LogLevel;
}