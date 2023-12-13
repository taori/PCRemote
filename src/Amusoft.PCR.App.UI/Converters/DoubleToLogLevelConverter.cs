using System.Globalization;
using Amusoft.PCR.Domain.UI.ValueTypes;

namespace Amusoft.PCR.App.UI.Converters;

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
				<= 0 or < 1 => LogEntryType.Trace
				, >= 1 and < 2 => LogEntryType.Debug
				, >= 2 and < 3 => LogEntryType.Information
				, >= 3 and < 4 => LogEntryType.Warning
				, >= 4 and < 5 => LogEntryType.Error
				, >= 5 or > 6 => LogEntryType.Fatal
				, _ => LogEntryType.Fatal
			};
		}

		return value;
	}
}