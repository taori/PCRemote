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
using AndroidX.Fragment.App;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.Maui.Platform;
using Fragment = AndroidX.Fragment.App.Fragment;
using IToast = Amusoft.PCR.Domain.Services.IToast;
using View = Android.Views.View;

namespace Amusoft.PCR.App.UI.Implementations;

internal class Toast : IToast
{
	public IToastable Make(string text, bool shortDuration = true, double textSize = 14)
	{
		var fragment = new ToastFragment(DateTime.Now);
		var duration = shortDuration
			? TimeSpan.FromMilliseconds(1500)
			: TimeSpan.FromMilliseconds(3500);

		fragment.SetTextSize(textSize);
		fragment.SetDuration(duration);

		return fragment;
	}
}

public class ToastFragment : Fragment, IToastable
{
	private readonly DateTime _token;
	private System.Timers.Timer _timer;

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
				HideFragment();
				_timer.Enabled = false;
				_timer.Stop();
			}
		}
		else
		{
			_timer.Enabled = false;
			_timer.Stop();
		}
	}

	// public override void OnCreate(Bundle? savedInstanceState)
	// {
	// 	base.OnCreate(savedInstanceState);
	// 	// if(Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu && Platform.CurrentActivity.OnBackInvokedDispatcher is {} dispatcher)
	// 	// 	dispatcher.RegisterOnBackInvokedCallback(0, this);
	// 	
	// 	// this.Activity?.OnBackPressedDispatcher.AddCallback(new BackPressedActionDelegate(OnBackInvoked));
	// }

	public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
	{
		return inflater.Inflate(Resource.Layout.toast_fragment, container, false);
	}

	// public void OnBackInvoked()
	// {
	// 	// if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu && Platform.CurrentActivity.OnBackInvokedDispatcher is { } dispatcher)
	// 	// 	dispatcher.UnregisterOnBackInvokedCallback(this);
	// 	HideFragment();
	// }

	private void HideFragment()
	{
		if (this.Context?.GetFragmentManager() is { } fm)
		{
			fm.PopBackStack(GetFragmentId(), (int) PopBackStackFlags.Inclusive);
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
		return this;
	}

	public IToastable SetPosition(Position value, int xOffset = 0, int yOffset = 0)
	{
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
			transaction.AddToBackStack(GetFragmentId());
			transaction.Add(Android.Resource.Id.Content, this);
			transaction.Commit();
		}
	}

	private DateTime? _hideAt;
	private double _textSize;
	private TimeSpan _duration;

	private void UpdateLease(DateTime leaseUntil)
	{
		_hideAt = leaseUntil;
		_timer.Enabled = true;
		_timer.Start();
	}

	public IToastable SetTextSize(double textSize)
	{
		_textSize = textSize;
		return this;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_timer.Elapsed -= CheckIfFragmentMustBeClosed;
			_timer.Dispose();
		}

		base.Dispose(disposing);
	}
}