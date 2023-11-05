using Amusoft.PCR.Application.Resources;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.Shared;

namespace Amusoft.PCR.Application.UI.VM;

public class SystemStateViewModel : PageViewModel
{
	public SystemStateViewModel(ITypedNavigator navigator) : base(navigator)
	{
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.Navi_SystemState;
	}
}