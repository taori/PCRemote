﻿using Amusoft.PCR.Domain.Services;
using CommunityToolkit.Maui.Core;
using IToast = Amusoft.PCR.Domain.Services.IToast;

namespace Amusoft.PCR.App.UI.Implementations;

internal class Toast : IToast
{
	public IToastable Make(string text, bool shortDuration = true, double textSize = 14)
	{
		return new Toastable(CommunityToolkit.Maui.Alerts.Toast.Make(text, shortDuration ? ToastDuration.Short : ToastDuration.Long, textSize));
	}
}

internal class Toastable : IToastable
{
	private readonly CommunityToolkit.Maui.Core.IToast _toast;

	internal Toastable(CommunityToolkit.Maui.Core.IToast toast)
	{
		_toast = toast;
	}

	public void Dispose()
	{
		_toast.Dispose();
	}

	public Task Show()
	{
		return _toast.Show();
	}

	public Task Dismiss()
	{
		return _toast.Dismiss();
	}

	// public string Text
	// {
	// 	get => _toast.Text;
	// 	set => _toast.Text = value;
	// }
}