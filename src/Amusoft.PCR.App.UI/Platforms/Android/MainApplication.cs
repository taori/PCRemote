using Amusoft.PCR.App.UI;
using Android.App;
using Android.Runtime;

namespace Amusoft.PCR.UI.App;

[Application(UsesCleartextTraffic = true)]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}