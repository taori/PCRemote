
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.CoordinatorLayout.Widget;
using View = Android.Views.View;

// ReSharper disable once CheckNamespace
namespace Amusoft.PCR.App.UI.Controls;

public partial class TrackerViewHandler
{
	private VelocityTracker? _velocityTracker;
	
	protected override CoordinatorLayout CreatePlatformView()
	{
		var root = new CoordinatorLayout(Context)
		{
			Background = Context.GetDrawable(Android.Resource.Drawable.DarkHeader),
			LayoutParameters = new CoordinatorLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
		};
		return root;
	}

	protected override void ConnectHandler(CoordinatorLayout platformView)
	{
		base.ConnectHandler(platformView);
		platformView.Touch += PlatformViewOnTouch;
	}

	private void PlatformViewOnTouch(object? sender, View.TouchEventArgs e)
	{
		if (e is {Event.Action: MotionEventActions.Up})
			VirtualView.TapCommand?.Execute(VirtualView.TapCommandParameter);
	}

	protected override void DisconnectHandler(CoordinatorLayout platformView)
	{
		platformView.Touch -= PlatformViewOnTouch;
		base.DisconnectHandler(platformView);
	}

	public static void TapCommandRequested(TrackerViewHandler arg1, ITrackerView arg2, object? arg3)
	{
	}

	public static void BackgroundChanged(TrackerViewHandler arg1, ITrackerView arg2)
	{
		arg1.PlatformView.Background = arg2.Background?.ToDrawable(MauiApplication.Current);
	}

	public static void BackgroundColorChanged(TrackerViewHandler arg1, ITrackerView arg2)
	{
	}

	public static void TapEnabledChanged(TrackerViewHandler arg1, ITrackerView arg2) { }

	public static void TapCommandParameterChanged(TrackerViewHandler arg1, ITrackerView arg2) { }

	public static void TapCommandChangedChanged(TrackerViewHandler arg1, ITrackerView arg2) { }

	public static void SensitivityChanged(TrackerViewHandler arg1, ITrackerView arg2)
	{
	}
}