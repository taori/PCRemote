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
}
