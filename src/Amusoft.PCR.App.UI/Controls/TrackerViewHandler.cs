using Microsoft.Maui.Handlers;
using System.Windows.Input;

#if IOS || MACCATALYST
using PlatformView = VideoDemos.Platforms.MaciOS.MauiVideoPlayer;
#elif ANDROID
using PlatformView = AndroidX.CoordinatorLayout.Widget.CoordinatorLayout;
#elif WINDOWS
using PlatformView = Microsoft.UI.Xaml.Controls.Grid;
#elif (NETSTANDARD || !PLATFORM) || (NET6_0_OR_GREATER && !IOS && !ANDROID)
using PlatformView = System.Object;
#endif

namespace Amusoft.PCR.App.UI.Controls;

public partial class TrackerViewHandler : ViewHandler<ITrackerView, PlatformView>
{
	public TrackerViewHandler(IPropertyMapper<ITrackerView, TrackerViewHandler> mapper) : base(mapper, CommandMapper)
	{
	}

	public TrackerViewHandler() : base(PropertyMapper, CommandMapper)
	{
	}

	public static IPropertyMapper<ITrackerView, TrackerViewHandler> PropertyMapper = new PropertyMapper<ITrackerView, TrackerViewHandler>(ViewMapper)
	{
		[nameof(TrackerView.Background)] = BackgroundChanged,
		[nameof(TrackerView.BackgroundColor)] = BackgroundColorChanged,
		[nameof(TrackerView.TapCommand)] = TapCommandChangedChanged,
		[nameof(TrackerView.TapCommandParameter)] = TapCommandParameterChanged,
		[nameof(TrackerView.TapEnabled)] = TapEnabledChanged,
		[nameof(TrackerView.Sensitivity)] = SensitivityChanged,
	};

	public static CommandMapper<ITrackerView, TrackerViewHandler> CommandMapper = new(ViewCommandMapper)
	{
		[nameof(TrackerView.TapCommand)] = TapCommandRequested
	};

	// public static void TapCommandRequested(TrackerViewHandler arg1, ITrackerView arg2, object? arg3)
	// {
	// }
	//
	// public static void BackgroundChanged(TrackerViewHandler arg1, ITrackerView arg2)
	// {
	// 	throw new NotImplementedException();
	// }
	//
	// public static void BackgroundColorChanged(TrackerViewHandler arg1, ITrackerView arg2)
	// {
	// 	throw new NotImplementedException();
	// }
	//
	// public static void TapEnabled(TrackerViewHandler arg1, ITrackerView arg2){ }
	//
	// public static void TapCommandParameterChanged(TrackerViewHandler arg1, ITrackerView arg2) { }
	//
	// public static void TapCommandChangedChanged(TrackerViewHandler arg1, ITrackerView arg2) { }

	// private static void SensitivityChanged(TrackerViewHandler arg1, ITrackerView arg2)
	// {
	// 	throw new NotImplementedException();
	// }

}

public interface ITrackerView : IView
{
	bool TapEnabled { get; set; }
	ICommand? TapCommand { get; set; }
	object? TapCommandParameter { get; set; }
	int Sensitivity { get; set; }
}