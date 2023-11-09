using Amusoft.PCR.Domain.Services;
using Android.App;
using Android.OS;
using Android.OS.Ext;
using Android.Views;
using Android.Widget;
using Android.Window;
using AndroidX.Activity;
using AndroidX.Core.View;
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
		var fragmentManager = Platform.CurrentActivity?.GetFragmentManager();
		var transaction = fragmentManager.BeginTransaction();
		transaction.AddToBackStack(fragment.GetFragmentId());
		transaction.Add(Android.Resource.Id.Content, fragment);
		transaction.Commit();
		return fragment;
		// return new Toastable(Android.Widget.Toast.MakeText(Platform.AppContext, text, shortDuration ? ToastLength.Short : ToastLength.Long)!);
	}
}

public class BackPressedActionDelegate : OnBackPressedCallback
{
	private readonly Action _callback;

	public BackPressedActionDelegate(Action callback) : base(true)
	{
		_callback = callback;
	}

	public override void HandleOnBackPressed()
	{
		this.Enabled = false;
		_callback.Invoke();
		this.Dispose(true);
	}
}

public class ToastFragment : Fragment, IToastable, IOnBackInvokedCallback
{
	private readonly DateTime _token;

	public ToastFragment(DateTime token)
	{
		_token = token;
	}

	public override void OnCreate(Bundle? savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		if(Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu && Platform.CurrentActivity.OnBackInvokedDispatcher is {} dispatcher)
			dispatcher.RegisterOnBackInvokedCallback(0, this);
		
		// this.Activity?.OnBackPressedDispatcher.AddCallback(new BackPressedActionDelegate(OnBackInvoked));
	}

	public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
	{
		return inflater.Inflate(Resource.Layout.toast_fragment, container, false);
	}

	public void OnBackInvoked()
	{
		if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu && Platform.CurrentActivity.OnBackInvokedDispatcher is { } dispatcher)
			dispatcher.UnregisterOnBackInvokedCallback(this);
		if (this.Context?.GetFragmentManager() is { } fm)
		{
			fm.PopBackStack(GetFragmentId(), (int)PopBackStackFlags.Inclusive);
		}
	}

	public Task Show()
	{
		return Task.CompletedTask;
	}

	public Task Dismiss()
	{
		return Task.CompletedTask;
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
}

internal class Toastable : IToastable
{
	private readonly Android.Widget.Toast _toast;

	internal Toastable(Android.Widget.Toast toast)
	{
		_toast = toast;
	}

	public void Dispose()
	{
		_toast.Dispose();
	}

	public Task Show()
	{
		_toast.Show();
		return Task.CompletedTask;
	}

	public Task Dismiss()
	{
		_toast.Cancel();
		return Task.CompletedTask;
	}

	public IToastable SetText(string value)
	{
		_toast.SetText(value);
		return this;
	}

	public IToastable SetPosition(Position value, int xOffset = 0, int yOffset = 0)
	{
		var gravity = value switch
		{
			Position.Bottom => GravityFlags.Bottom,
			Position.Top => GravityFlags.Top,
			Position.Left => GravityFlags.Left,
			Position.Right => GravityFlags.Right,
			_ => GravityFlags.Bottom
		};

		_toast.SetGravity(gravity, xOffset, yOffset);
		return this;
	}
}