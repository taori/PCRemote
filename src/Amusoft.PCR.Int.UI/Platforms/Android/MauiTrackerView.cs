using Android.Content;
using Android.Views;
using AndroidX.CoordinatorLayout.Widget;
using System.Numerics;

// ReSharper disable once CheckNamespace
namespace Amusoft.PCR.Int.UI;

public class MauiTrackerView : CoordinatorLayout
{
	private TrackerView? _virtualView;
	private VelocityTracker? _velocityTracker;
	private DateTime _downTime = DateTime.Now;
	private int _sensitivity = 1000;

	public MauiTrackerView(TrackerView virtualView, Context context) : base(context)
	{
		_virtualView = virtualView;
		this.Background = Context?.GetDrawable(global::Android.Resource.Drawable.DarkHeader);
		LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);

		this.Touch += PlatformViewOnTouch;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			this.Touch -= PlatformViewOnTouch;

			_velocityTracker?.Recycle();
			_velocityTracker = null;
			_virtualView = null;
			_velocityTracker = null;
		}

		base.Dispose(disposing);
	}

	private void PlatformViewOnTouch(object? sender, TouchEventArgs e)
	{
		if (e is { Event.Action: MotionEventActions.Up })
			_virtualView?.TapCommand?.Execute(_virtualView.TapCommandParameter);
	}


	public override bool DispatchTouchEvent(MotionEvent? e)
	{
		if (e is null)
			return false;

		var index = e.ActionIndex;
		var action = e.ActionMasked;
		var pointerId = e.GetPointerId(index);

		switch (action & MotionEventActions.Mask)
		{
			case MotionEventActions.Pointer1Up:
				_virtualView?.MultiTapCommand?.Execute(null);
				break;
			case MotionEventActions.Up:
				if ((DateTime.Now - _downTime).TotalMilliseconds < 200)
					_virtualView?.TapCommand?.Execute(_virtualView.TapCommandParameter);
				break;
		}

		switch (action)
		{
			case MotionEventActions.Down:
				_downTime = DateTime.Now;

				if (_velocityTracker == null)
				{
					_velocityTracker = VelocityTracker.Obtain();
				}
				else
				{
					// Reset the velocity tracker back to its initial state.
					_velocityTracker.Clear();
				}

				if (IfVelocityTrackerIsNull())
					return true;

				_velocityTracker?.AddMovement(e);
				break;
			case MotionEventActions.Move:
				if (IfVelocityTrackerIsNull())
					return true;

				_velocityTracker?.AddMovement(e);
				_velocityTracker?.ComputeCurrentVelocity(_sensitivity);
				TryExportVelocity(_velocityTracker?.GetXVelocity(pointerId) ?? 0, _velocityTracker?.GetYVelocity(pointerId) ?? 0);

				break;
			case MotionEventActions.Up:
			case MotionEventActions.Cancel:
				if (IfVelocityTrackerIsNull())
					return true;

				_velocityTracker?.Recycle();
				_velocityTracker = null;
				break;
		}

		return true;
	}

	private void TryExportVelocity(float xVel, float yVel)
	{
		if (MathF.Abs(xVel) < 0.1f && MathF.Abs(yVel) < 0.1f)
			return;

		_virtualView?.VelocityOccuredCommand?.Execute(new Vector2(xVel, yVel));
	}

	private bool IfVelocityTrackerIsNull()
	{
		if (_velocityTracker == null)
		{
			return true;
		}

		return false;
	}

	public void UpdateSensitivity(TrackerView virtualView)
	{
		_sensitivity = virtualView.Sensitivity;
	}
}