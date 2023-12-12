using Amusoft.PCR.Domain.UI.Entities;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class LogDisplaySettingsViewModel : ObservableObject
{
	public LogDisplaySettingsViewModel(LogSettings settings)
	{
		_displayLogger = settings.DisplayLogger;
		_displayDate = settings.DisplayDate;
		_displayFullLoggerName = settings.DisplayFullLoggerName;
		_dateFormat = settings.DateFormat;
		_widthDate = settings.WidthDate;
		_widthLogger = settings.WidthLogger;
	}

	[ObservableProperty] [NotifyPropertyChangedFor(nameof(GridColumnDefinitions))]
	private bool _displayLogger;

	[ObservableProperty] [NotifyPropertyChangedFor(nameof(GridColumnDefinitions))]
	private bool _displayDate;

	[ObservableProperty] private bool _displayFullLoggerName;

	[ObservableProperty] private string _dateFormat;

	[ObservableProperty] private int _widthDate;

	[ObservableProperty] private int _widthLogger;

	public string GridColumnDefinitions => (_displayDate, _displayLogger) switch
	{
		(true, true) => $"{_widthDate},{_widthLogger},*",
		(false, false) => "0,0,*",
		(true, false) => $"{_widthDate},0,*",
		(false, true) => $"0,{_widthLogger},*",
	};

	// public int WidthDate { get; set; } = 140;
	//
	// public int WidthLogger { get; set; } = 140;
}