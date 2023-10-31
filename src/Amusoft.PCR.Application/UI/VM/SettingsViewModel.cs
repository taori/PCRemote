using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.Shared;

namespace Amusoft.PCR.Application.UI.VM;

public partial class SettingsViewModel : PageViewModel
{
    protected override string GetDefaultPageTitle()
    {
        return Resources.Translations.Page_Title_Settings;
    }

    public SettingsViewModel(ITypedNavigator navigator) : base(navigator)
    {
    }
}