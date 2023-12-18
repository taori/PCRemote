using Amusoft.PCR.AM.UI.Interfaces;
using Android.Content;
using Google.Android.Material.TimePicker;
using Microsoft.Maui.Platform;
using Object = Java.Lang.Object;
using View = Android.Views.View;

namespace Amusoft.PCR.Int.UI.Shared;

internal partial class UserInterfaceService : IUserInterfaceService
{
	private readonly IAndroidResourceBridge _resourceBridge;

	public UserInterfaceService(IAndroidResourceBridge resourceBridge)
	{
		_resourceBridge = resourceBridge;
	}

	public Task<TimeSpan?> GetTimeFromPickerAsync(string title, TimeSpan intialTime)
	{
		if (Microsoft.Maui.ApplicationModel.Platform.CurrentActivity is null)
			return Task.FromResult((TimeSpan?)TimeSpan.FromMinutes(1));
		
		var tcs = new TaskCompletionSource<TimeSpan?>();

		var timePicker = new MaterialTimePicker.Builder()
			.SetTitleText(title)
			.SetInputMode(MaterialTimePicker.InputModeClock)
			.SetHour(intialTime.Hours)
			.SetMinute(intialTime.Minutes)
			.SetTimeFormat(TimeFormat.Clock24h)
			.Build();

		tcs.Task.ContinueWith(t => { timePicker.Dispose(); });

		timePicker.AddOnNegativeButtonClickListener(new OnClickListener(() => tcs.TrySetResult(null)));
		timePicker.AddOnCancelListener(new DialogInterfaceOnCancelListener(() => { tcs.TrySetResult(null); }));
		timePicker.AddOnDismissListener(new DialogInterfaceOnCancelListener(() => { tcs.TrySetResult(null); }));
		timePicker.AddOnPositiveButtonClickListener(new OnClickListener(() =>
		{
			var target = new TimeSpan(timePicker.Hour, timePicker.Minute, 0);
			if (target.Equals(TimeSpan.Zero))
				target = intialTime;

			tcs.TrySetResult(target);
		}));
		
		if (Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.GetFragmentManager() is { } fm)
			timePicker.Show(fm, null);
		
		return tcs.Task;
	}

	private class OnClickListener : Object, View.IOnClickListener
	{
		private readonly Action _callback;

		public OnClickListener(Action callback)
		{
			_callback = callback;
		}

		public void OnClick(View? v)
		{
			_callback.Invoke();
		}
	}


	private class DialogInterfaceOnCancelListener : Object, IDialogInterfaceOnCancelListener, IDialogInterfaceOnDismissListener
	{
		private readonly Action _callback;

		public DialogInterfaceOnCancelListener(Action callback)
		{
			_callback = callback;
		}

		public void OnCancel(IDialogInterface? dialog)
		{
			_callback?.Invoke();
		}

		public void OnDismiss(IDialogInterface? dialog)
		{
			_callback?.Invoke();
		}
	}
}