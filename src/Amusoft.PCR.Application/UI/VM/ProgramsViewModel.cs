using Amusoft.PCR.Application.Resources;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.Shared;

namespace Amusoft.PCR.Application.UI.VM;

public class ProgramsViewModel : PageViewModel
{
	public ProgramsViewModel(ITypedNavigator navigator) : base(navigator)
	{
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.Nav_Programs;
	}
}