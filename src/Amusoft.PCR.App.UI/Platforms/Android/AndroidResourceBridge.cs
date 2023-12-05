using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;
using Resource = Amusoft.PCR.App.UI.Resource;

namespace Amusoft.PCR.UI.App;

public class AndroidResourceBridge : IAndroidResourceBridge
{
	public int TimePickerDialogTheme => Resource.Style.TimeSpinnerDialogTheme;
	public string MessageAbort => Translations.Generic_Abort;
	public string MessageHibernate_0 => Translations.Generic_Hibernate_0;
	public string MessageShutdown_0 => Translations.Generic_Shutdown_0;
	public string MessageRestart_0 => Translations.Generic_Restart_0;
}