using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class DebugViewModel : PageViewModel
{
	private readonly IToast _toast;
	private readonly IToastable _toastInstance;

	public DebugViewModel(ITypedNavigator navigator, IToast toast) : base(navigator)
	{
		_toast = toast;
		_toastInstance = _toast.Make(string.Empty).SetPosition(Position.Top);
	}

	[RelayCommand]
	public Task DisplayToast()
	{
		return _toastInstance
			.SetText(DateTime.Now.ToString("G"))
			.Show();
	}

	protected override string GetDefaultPageTitle()
	{
		return "Debug";
	}
}