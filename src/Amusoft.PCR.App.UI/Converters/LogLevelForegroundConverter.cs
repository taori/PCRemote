using System.Globalization;
using Amusoft.PCR.Domain.UI.ValueTypes;

namespace Amusoft.PCR.App.UI.Converters;

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