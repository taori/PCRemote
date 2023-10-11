using Amusoft.PCR.Application.Shared;

namespace Amusoft.PCR.Application.UI;

public partial class SettingsViewModel : PageViewModel
{
	protected override string GetDefaultPageTitle()
	{
		return Resources.Translations.Page_Title_Settings;
	}
}