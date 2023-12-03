
// ReSharper disable once CheckNamespace
namespace Amusoft.PCR.Int.UI;

public partial class TrackerViewHandler
{
	protected override MauiTrackerView CreatePlatformView()
	{
		return new MauiTrackerView();
	}
}