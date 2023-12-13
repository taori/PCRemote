using System.Globalization;
using Amusoft.PCR.Domain.UI.ValueTypes;

namespace Amusoft.PCR.App.UI.Converters;

public class LogLevelSingleCharConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is LogEntryType enumType)
			return enumType switch
			{
				LogEntryType.Trace => "V"
				, LogEntryType.Debug => "D"
				, LogEntryType.Information => "I"
				, LogEntryType.Warning => "W"
				, LogEntryType.Error => "E"
				, LogEntryType.Fatal => "F"
				, _ => throw new ArgumentOutOfRangeException()
			};

		return "?";
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}

public class LogLevelForegroundConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is not LogEntryType enumType)
			return null;

		if (Application.Current is not null)
		{
			switch (Application.Current.RequestedTheme)
			{
				case AppTheme.Unspecified:
				case AppTheme.Light:
					return enumType switch
					{
						LogEntryType.Trace => Colors.DarkGrey
						, LogEntryType.Debug => Colors.LightGray
						, LogEntryType.Information => Colors.Green
						, LogEntryType.Warning => Colors.Yellow
						, LogEntryType.Error => Colors.Red
						, LogEntryType.Fatal => Colors.DarkRed
						, _ => throw new ArgumentOutOfRangeException()
					};
					break;
				case AppTheme.Dark:
					return enumType switch
					{
						LogEntryType.Trace => Colors.DarkGrey
						, LogEntryType.Debug => Colors.LightGray
						, LogEntryType.Information => Colors.Green
						, LogEntryType.Warning => Colors.Yellow
						, LogEntryType.Error => Colors.Red
						, LogEntryType.Fatal => Colors.DarkRed
						, _ => throw new ArgumentOutOfRangeException()
					};
			}
		}

		return null;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}

public class LogLevelBackgroundConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		// Application.Current.RequestedTheme == 
		return null;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}

public class DoubleToLogLevelConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is LogEntryType d)
		{
			return (int)d;
		}

		return value;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is double d)
		{
			return d switch
			{
				<= 0 or < 1 => LogEntryType.Trace, >= 1 and < 2 => LogEntryType.Debug, >= 2 and < 3 => LogEntryType.Information, >= 3 and < 4 => LogEntryType.Warning
				, >= 4 and < 5 => LogEntryType.Error, >= 5 or > 6 => LogEntryType.Fatal, _ => LogEntryType.Fatal
			};
		}

		return value;
	}
}