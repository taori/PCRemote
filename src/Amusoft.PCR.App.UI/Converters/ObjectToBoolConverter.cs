using System.Globalization;
using CommunityToolkit.Maui.Converters;

namespace Amusoft.PCR.App.UI.Converters;

public class ObjectToBoolConverter<TObject> : BaseConverter<TObject?, bool>
{
	public override bool ConvertFrom(TObject? value, CultureInfo? culture)
	{
		return EqualityComparer<TObject>.Default.Equals(value, MatchAgainst) ? MatchTrueValue : MatchFalseValue;
	}

	public override TObject? ConvertBackTo(bool value, CultureInfo? culture)
	{
		throw new NotImplementedException();
	}

	public TObject MatchAgainst { get; set; }

	public bool MatchTrueValue { get; set; }

	public bool MatchFalseValue { get; set; }

	/// <inheritdoc/>
	public override bool DefaultConvertReturnValue { get; set; } = false;

	/// <inheritdoc/>
	public override TObject? DefaultConvertBackReturnValue { get; set; } = default;
}