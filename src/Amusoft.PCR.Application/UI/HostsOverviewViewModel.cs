using Amusoft.PCR.Application.Resources;

namespace Amusoft.PCR.Application.UI;

public partial class HostsOverviewViewModel : Shared.ReloadablePageViewModel
{
	protected override string GetDefaultPageTitle()
	{
		return Translations.Page_Title_HostsOverview;
	}
}