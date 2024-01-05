using System.Globalization;
using CommunityToolkit.Maui.Converters;

namespace Amusoft.PCR.App.UI.Converters;

public class ObjectToObjectConverter<T1, T2> : BaseConverter<T1?, T2?>
{
	public override T2 ConvertFrom(T1? value, CultureInfo? culture)
	{
		return EqualityComparer<T1>.Default.Equals(value, MatchAgainst) ? MatchTrueValue : MatchFalseValue;
	}

	public override T1? ConvertBackTo(T2 value, CultureInfo? culture)
	{
		throw new NotImplementedException();
	}

	public T1 MatchAgainst { get; set; }

	public T2 MatchTrueValue { get; set; }

	public T2 MatchFalseValue { get; set; }

	/// <inheritdoc/>
	public override T2? DefaultConvertReturnValue { get; set; } = default;

	/// <inheritdoc/>
	public override T1? DefaultConvertBackReturnValue { get; set; } = default;
}