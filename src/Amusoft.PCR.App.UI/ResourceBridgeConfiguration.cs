using Amusoft.PCR.Int.UI;

namespace Amusoft.PCR.App.UI;

public static class ResourceBridgeConfiguration
{
	public static void Apply()
	{
#if ANDROID
		ResourceBridge.ToastIcon = Resource.Mipmap.appicon_round;
#endif
	}
}