using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.Application.Shared;

public partial class NavigationItem : ObservableObject
{
	[ObservableProperty]
	private string? _text;

	[ObservableProperty]
	private string? _imagePath;

	[ObservableProperty]
	private IRelayCommand? _command;

	// workaround because Binding Path=. does not work in maui?
	public NavigationItem Self => this;
}

public class NavigationItem<T> : NavigationItem
{
	public NavigationItem(T value)
	{
		Value = value;
	}

	public T Value { get; set; }
}
