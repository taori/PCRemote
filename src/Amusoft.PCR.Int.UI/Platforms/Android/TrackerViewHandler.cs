
// ReSharper disable once CheckNamespace
namespace Amusoft.PCR.Int.UI;

public partial class TrackerViewHandler
{
	protected override void DisconnectHandler(MauiTrackerView platformView)
	{
		platformView.Dispose();
		base.DisconnectHandler(platformView);
	}

	protected override MauiTrackerView CreatePlatformView()
	{
		return new MauiTrackerView(VirtualView, Context);
	}
}