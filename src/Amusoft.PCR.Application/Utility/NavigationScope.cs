namespace Amusoft.PCR.Application.Utility;

public class NavigationScope : IDisposable
{
	public NavigationScope()
	{
		CurrentScope = this;
	}

	public static bool TryGetScope(out NavigationScope? scope)
	{
		scope = CurrentScope;
		return CurrentScope != null;
	}

	private static NavigationScope? CurrentScope;

	private List<object> _values = new();

	public void Push(object value) => _values.Add(value);

	public List<object> GetValues()
	{
		return _values;
	}

	public void Dispose()
	{
		_values.Clear();
		CurrentScope = null;
	}
}