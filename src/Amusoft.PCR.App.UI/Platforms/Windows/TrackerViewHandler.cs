
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml.Media;
using Grid = Microsoft.UI.Xaml.Controls.Grid;
using SolidColorBrush = Microsoft.UI.Xaml.Media.SolidColorBrush;

// ReSharper disable once CheckNamespace
namespace Amusoft.PCR.App.UI.Controls;

public partial class TrackerViewHandler
{
	protected override Microsoft.UI.Xaml.Controls.Grid CreatePlatformView()
	{
		return new Microsoft.UI.Xaml.Controls.Grid(){ Background = new SolidColorBrush(Windows.UI.Color.FromArgb(127,255,0,0))};
	}

	protected override void ConnectHandler(Grid platformView)
	{
		base.ConnectHandler(platformView);
	}

	protected override void DisconnectHandler(Grid platformView)
	{
		base.DisconnectHandler(platformView);
	}

	public static void TapCommandRequested(TrackerViewHandler arg1, ITrackerView arg2, object? arg3)
	{
	}
	
	public static void BackgroundChanged(TrackerViewHandler arg1, ITrackerView arg2)
	{
	}
	
	public static void BackgroundColorChanged(TrackerViewHandler arg1, ITrackerView arg2)
	{
	}
	
	public static void TapEnabledChanged(TrackerViewHandler arg1, ITrackerView arg2){ }
	
	public static void TapCommandParameterChanged(TrackerViewHandler arg1, ITrackerView arg2) { }
	
	public static void TapCommandChangedChanged(TrackerViewHandler arg1, ITrackerView arg2) { }

	public static void SensitivityChanged(TrackerViewHandler arg1, ITrackerView arg2)
	{
	}
}