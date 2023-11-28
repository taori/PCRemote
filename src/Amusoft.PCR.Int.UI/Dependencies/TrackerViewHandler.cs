using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

#if IOS || MACCATALYST
// using PlatformView = VideoDemos.Platforms.MaciOS.MauiVideoPlayer;
#elif ANDROID
using PlatformView = Amusoft.PCR.App.UI.Platforms.Android.MauiTrackerView;
#elif WINDOWS
using PlatformView = Amusoft.PCR.App.UI.Platforms.Windows.MauiTrackerView;
#elif (NETSTANDARD || !PLATFORM) || (NET6_0_OR_GREATER && !IOS && !ANDROID)
using PlatformView = System.Object;
#endif

namespace Amusoft.PCR.App.UI.Controls;

public partial class TrackerViewHandler : ViewHandler<TrackerView, PlatformView>
{
	public TrackerViewHandler(IPropertyMapper<TrackerView, TrackerViewHandler> mapper) : base(mapper, CommandMapper)
	{
	}

	public TrackerViewHandler() : base(PropertyMapper, CommandMapper)
	{
	}

	public static CommandMapper<TrackerView, TrackerViewHandler> CommandMapper = new(ViewCommandMapper)
	{
		[nameof(TrackerView.TapCommand)] = TapCommandRequested
	};

	public static IPropertyMapper<TrackerView, TrackerViewHandler> PropertyMapper = new PropertyMapper<TrackerView, TrackerViewHandler>(ViewMapper)
	{
		[nameof(TrackerView.Background)] = BackgroundChanged,
		[nameof(TrackerView.BackgroundColor)] = BackgroundColorChanged,
		[nameof(TrackerView.TapCommand)] = TapCommandChangedChanged,
		[nameof(TrackerView.TapCommandParameter)] = TapCommandParameterChanged,
		[nameof(TrackerView.TapEnabled)] = TapEnabledChanged,
		[nameof(TrackerView.Sensitivity)] = SensitivityChanged,
	};

	private static void TapEnabledChanged(TrackerViewHandler handler, TrackerView virtualView)
	{
	}

	private static void TapCommandRequested(TrackerViewHandler handler, TrackerView virtualView, object? arg3)
	{
	}
	
	private static void BackgroundChanged(TrackerViewHandler handler, TrackerView virtualView)
	{
		handler.PlatformView.UpdateBackground(virtualView);
	}
	
	private static void BackgroundColorChanged(TrackerViewHandler handler, TrackerView virtualView)
	{
		handler.PlatformView.UpdateBackground(virtualView);
	}
	
	private static void TapEnabled(TrackerViewHandler handler, TrackerView virtualView){ }
	
	private static void TapCommandParameterChanged(TrackerViewHandler handler, TrackerView virtualView) { }

	private static void TapCommandChangedChanged(TrackerViewHandler handler, TrackerView virtualView) { }

	private static void SensitivityChanged(TrackerViewHandler handler, TrackerView virtualView)
	{
		handler.PlatformView.UpdateSensitivity(virtualView);
	}
}