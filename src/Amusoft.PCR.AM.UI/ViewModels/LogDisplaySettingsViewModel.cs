using Amusoft.PCR.Domain.UI.Entities;
using Amusoft.PCR.Domain.UI.ValueTypes;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class LogDisplaySettingsViewModel : ObservableObject, IEquatable<LogDisplaySettingsViewModel>
{
	[ObservableProperty]
	private string _dateFormat;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(GridColumnDefinitions))]
	private bool _displayDate;

	[ObservableProperty]
	private bool _displayFullLoggerName;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(GridColumnDefinitions))]
	private bool _displayLogger;

	[ObservableProperty]
	private LogEntryType _logLevel;

	[ObservableProperty]
	private int _widthDate;

	[ObservableProperty]
	private int _widthLogger;

	public LogDisplaySettingsViewModel(LogSettings settings)
	{
		_displayLogger = settings.DisplayLogger;
		_displayDate = settings.DisplayDate;
		_displayFullLoggerName = settings.DisplayFullLoggerName;
		_dateFormat = settings.DateFormat;
		_widthDate = settings.WidthDate;
		_widthLogger = settings.WidthLogger;
		_logLevel = settings.LogLevel;
	}

	public string GridColumnDefinitions => (_displayDate, _displayLogger) switch
	{
		(true, true) => $"{_widthDate},{_widthLogger},*", (false, false) => "0,0,*", (true, false) => $"{_widthDate},0,*", (false, true) => $"0,{_widthLogger},*",
	};

	public bool Equals(LogDisplaySettingsViewModel? other)
	{
		if (ReferenceEquals(null, other))
			return false;
		if (ReferenceEquals(this, other))
			return true;
		return _dateFormat == other._dateFormat && _displayDate == other._displayDate && _displayFullLoggerName == other._displayFullLoggerName && _displayLogger == other._displayLogger &&
		       _widthDate == other._widthDate && _widthLogger == other._widthLogger && _logLevel == other._logLevel;
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj))
			return false;
		if (ReferenceEquals(this, obj))
			return true;
		if (obj.GetType() != this.GetType())
			return false;
		return Equals((LogDisplaySettingsViewModel)obj);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(_dateFormat, _displayDate, _displayFullLoggerName, _displayLogger, _widthDate, _widthLogger, _logLevel);
	}

	public LogSettings ToModel()
	{
		return new LogSettings()
		{
			DisplayLogger = _displayLogger
			, DisplayDate = _displayDate
			, DisplayFullLoggerName = _displayFullLoggerName
			, DateFormat = _dateFormat
			, WidthDate = _widthDate
			, WidthLogger = _widthLogger
			, LogLevel = _logLevel
		};
	}
}