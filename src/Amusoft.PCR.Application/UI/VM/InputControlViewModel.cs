using Amusoft.PCR.Application.Resources;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.Shared;

namespace Amusoft.PCR.Application.UI.VM;

public class InputControlViewModel : PageViewModel
{
	public InputControlViewModel(ITypedNavigator navigator) : base(navigator)
	{
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.Nav_Input;
	}
}