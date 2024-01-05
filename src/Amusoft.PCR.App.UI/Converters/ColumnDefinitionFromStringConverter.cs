using System.Globalization;

namespace Amusoft.PCR.App.UI.Converters;

public class ColumnDefinitionFromStringConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (targetType == typeof(ColumnDefinitionCollection) && value is string s)
		{
			var values = s.Split(",").Select(ParseStringValueAsDefinition);
			return new ColumnDefinitionCollection(values.ToArray());
		}

		return null;
	}

	private ColumnDefinition ParseStringValueAsDefinition(string s)
	{
		if (s.Equals("*"))
			return new ColumnDefinition(GridLength.Star);
		if (s.Equals("Auto"))
			return new ColumnDefinition(GridLength.Auto);
		if (double.TryParse(s, out var parsed))
			return new ColumnDefinition(new GridLength(parsed, GridUnitType.Absolute));

		throw new XamlParseException($"Failed to parse {s} as a ColumnDefinition");
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}