using Amusoft.PCR.Application.Resources;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.Shared;
using Amusoft.PCR.Domain.Services;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.Application.UI.VM;

public partial class InputControlViewModel : PageViewModel
{
	private readonly IToast _toast;

	public InputControlViewModel(ITypedNavigator navigator, IToast toast) : base(navigator)
	{
		_toast = toast;
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.Nav_Input;
	}

	[RelayCommand]
	private Task ViewTapped()
	{
		return _toast.Make("It works").Show();
	}
}