using System.Timers;
using Amusoft.PCR.Domain.Services;
using Android.App;
using Android.OS;
using Android.OS.Ext;
using Android.Views;
using Android.Widget;
using Android.Window;
using AndroidX.Activity;
using AndroidX.Core.View;
using AndroidX.Core.Widget;
using AndroidX.Fragment.App;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.Maui.Platform;
using Debug = System.Diagnostics.Debug;
using Fragment = AndroidX.Fragment.App.Fragment;
using IToast = Amusoft.PCR.Domain.Services.IToast;
using View = Android.Views.View;

namespace Amusoft.PCR.App.UI.Implementations;

internal class Toast : IToast
{
	public IToastable Make(string text, bool shortDuration = true, double textSize = 14)
	{
		// _ = CommunityToolkit.Maui.Alerts.Toast
		// 	.Make(text, shortDuration ? ToastDuration.Short : ToastDuration.Long)
		// 	.Show();
		
		var fragment = new ToastFragment(DateTime.Now);
		var duration = shortDuration
			? TimeSpan.FromMilliseconds(1500)
			: TimeSpan.FromMilliseconds(3500);

		fragment.SetText(text);
		fragment.SetTextSize(textSize);
		fragment.SetDuration(duration);

		return fragment;
	}
}

public class ToastFragment : Fragment, IToastable
{
	private readonly DateTime _token;
	private System.Timers.Timer _timer;

	private DateTime? _hideAt;
	private double _textSize;
	private TimeSpan _duration;
	private string? _text;
	private (Position value, int xOffset, int yOffset) _position;

	public ToastFragment(DateTime token)
	{
		_token = token;
		_timer = new(TimeSpan.FromMilliseconds(500));
		_timer.AutoReset = true;
		_timer.Elapsed += CheckIfFragmentMustBeClosed;
	}

	private void CheckIfFragmentMustBeClosed(object? sender, ElapsedEventArgs e)
	{
		if (_hideAt is { } hideAt)
		{
			if (Math.Abs((DateTime.Now - hideAt).TotalMilliseconds) < 50)
			{
				// Debug.WriteLine("CheckIfFragmentMustBeClosed - Hiding fragment");

				_timer.Enabled = false;
				_timer.Stop();
				HideFragment();
			}
		}
		else
		{
			// Debug.WriteLine("CheckIfFragmentMustBeClosed - stopping timer");
			_timer.Enabled = false;
			_timer.Stop();
		}
	}

	public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
	{
		return inflater.Inflate(Resource.Layout.toast_fragment, container, false);
	}

	public override void OnViewCreated(View view, Bundle? savedInstanceState)
	{
		UpdateEverything(view);
		base.OnViewCreated(view, savedInstanceState);
	}

	private void UpdateEverything(View view)
	{
		UpdatePosition(view);
		UpdateTextView(view);
		UpdateImageView(view);
	}

	private static void UpdateImageView(View view)
	{
		if (view.FindViewById<ImageView>(Resource.Id.imageView1) is { } imageView)
		{
			imageView.SetImageResource(Resource.Mipmap.appicon_round);
			if (imageView.LayoutParameters is ViewGroup.MarginLayoutParams { } ivMarginParams)
			{
				var l = Platform.AppContext.Resources?.DisplayMetrics?.Density * -10 ?? 0;
				var r = Platform.AppContext.Resources?.DisplayMetrics?.Density * -10 ?? 0;
				ivMarginParams.SetMargins((int) l, 0, (int) r, 0);
			}
		}
	}

	private void UpdatePosition(View view)
	{
		if (view is LinearLayout linearLayout)
		{
			var gravity = _position.value switch
			{
				Position.Top => GravityFlags.Top | GravityFlags.CenterHorizontal,
				Position.Left => GravityFlags.Left | GravityFlags.CenterVertical,
				Position.Right => GravityFlags.Right | GravityFlags.CenterVertical,
				Position.Center => GravityFlags.Center,
				Position.Bottom => GravityFlags.Bottom | GravityFlags.CenterHorizontal,
				_ => GravityFlags.Bottom | GravityFlags.CenterHorizontal
			};

			linearLayout.SetGravity(gravity);
		}
	}

	private void UpdateTextView(View? view)
	{
		if (view?.FindViewById<TextView>(Resource.Id.textView1) is { } textView)
		{
			textView.Text = _text ?? "undefined";
			textView.TextSize = (float) _textSize;
		}
	}

	public Task Show()
	{
		DisplayFragment(DateTime.Now.Add(_duration));
		return Task.CompletedTask;
	}

	public Task Dismiss()
	{
		HideFragment();
		return Task.CompletedTask;
	}

	public IToastable SetDuration(TimeSpan value)
	{
		_duration = value;
		return this;
	}

	public IToastable SetText(string value)
	{
		_text = value;
		UpdateTextView(View);
		return this;
	}

	public IToastable SetPosition(Position value, int xOffset = 0, int yOffset = 0)
	{
		_position = (value, xOffset, yOffset);
		return this;
	}

	public string GetFragmentId()
	{
		return $"toast-{_token.Ticks}";
	}

	public void DisplayFragment(DateTime dateTime)
	{
		var fragmentManager = Platform.CurrentActivity?.GetFragmentManager();
		if (fragmentManager is null)
			return;
		
		UpdateLease(dateTime);

		if (fragmentManager.FindFragmentByTag(GetFragmentId()) is null)
		{
			var transaction = fragmentManager.BeginTransaction();
			if (this.IsAdded)
			{
				// Debug.WriteLine($"ToastFragment show only 1");
				transaction
					.Show(this)
					.AddToBackStack(GetFragmentId())
					.Commit();
			}
			else
			{
				// Debug.WriteLine($"ToastFragment add+show");
				transaction
					.Add(Android.Resource.Id.Content, this, GetFragmentId())
					.AddToBackStack(GetFragmentId())
					.Commit();
			}
		}
		else
		{
			// Debug.WriteLine($"ToastFragment {GetFragmentId()} show only 2");
			// Debug.WriteLine($"{IsVisible} {IsHidden} {IsAdded}");
			if (!this.IsVisible)
			{
				var transaction = fragmentManager.BeginTransaction();
				transaction
					.Show(this)
					.AddToBackStack(GetFragmentId())
					.Commit();
			}
		}
	}

	private void HideFragment()
	{
		// Debug.WriteLine("HideFragment called");
		if (this.Context?.GetFragmentManager() is { } fm)
		{
			var transaction = fm.BeginTransaction();
			transaction.Hide(this);
			transaction.AddToBackStack(GetFragmentId());
			transaction.Commit();
		}
	}

	private void UpdateLease(DateTime leaseUntil)
	{
		_hideAt = leaseUntil;
		_timer.Enabled = true;
		_timer.Stop();
		_timer.Start();
	}

	public IToastable SetTextSize(double textSize)
	{
		_textSize = textSize;
		UpdateTextView(View);
		return this;
	}

	protected override void Dispose(bool disposing)
	{
		// Debug.WriteLine("ToastFragment disposing");
		if (disposing)
		{
			_timer.Elapsed -= CheckIfFragmentMustBeClosed;
			_timer.Dispose();
		}

		base.Dispose(disposing);
	}
}