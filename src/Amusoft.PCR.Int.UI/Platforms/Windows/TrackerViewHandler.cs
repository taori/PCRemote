
using Amusoft.PCR.App.UI.Platforms.Windows;

// ReSharper disable once CheckNamespace
namespace Amusoft.PCR.App.UI.Controls;

public partial class TrackerViewHandler
{
	protected override MauiTrackerView CreatePlatformView()
	{
		return new MauiTrackerView();
	}
}