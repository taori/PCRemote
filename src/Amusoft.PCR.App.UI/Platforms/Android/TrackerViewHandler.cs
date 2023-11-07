
using Amusoft.PCR.App.UI.Platforms.Android;
using Microsoft.Maui.Controls.Platform;

// ReSharper disable once CheckNamespace
namespace Amusoft.PCR.App.UI.Controls;

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