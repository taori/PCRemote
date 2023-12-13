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