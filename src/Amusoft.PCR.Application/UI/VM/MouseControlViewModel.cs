﻿using System.Diagnostics;
using System.Formats.Asn1;
using System.Numerics;
using System.Threading.Channels;
using Amusoft.PCR.Application.Extensions;
using Amusoft.PCR.Application.Resources;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.Shared;
using Amusoft.PCR.Application.UI.Repos;
using Amusoft.PCR.Application.Utility;
using Amusoft.PCR.Domain.Services;
using Amusoft.PCR.Domain.VM;
using Amusoft.PCR.Int.IPC;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Google.Protobuf.WellKnownTypes;

namespace Amusoft.PCR.Application.UI.VM;

public partial class MouseControlViewModel : PageViewModel, INavigationCallbacks
{
	private readonly HostViewModel _host;
	private readonly ClientSettingsRepository _settingsRepository;
	private readonly IToast _toast;

	private CancellationTokenSource? _moveCts = new();

	private readonly Channel<(int x, int y)> _mouseMoveChannel;

	private readonly ChannelStreamReader<(int x, int y)> _streamReader;

	public MouseControlViewModel(ITypedNavigator navigator, HostViewModel host, ClientSettingsRepository settingsRepository, IToast toast) : base(navigator)
	{
		_host = host;
		_settingsRepository = settingsRepository;
		_toast = toast;
		_mouseMoveChannel = Channel.CreateUnbounded<(int x, int y)>();
		_streamReader = new ChannelStreamReader<(int x, int y)>(_mouseMoveChannel);
		_toastable = _toast.Make("");
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.InputControl_MouseControl;
	}

	[ObservableProperty]
	private bool _toggle;

	[ObservableProperty]
	private int _sensitivity = 20;

	private readonly IToastable _toastable;

	[RelayCommand]
	private void VelocityChanged(Vector2 vector)
	{
		_ = _mouseMoveChannel.Writer.WriteAsync(((int)vector.X, (int)vector.Y));
	}

	public Task OnNavigatedAwayAsync()
	{
		_moveCts?.Dispose();
		return Task.CompletedTask;
	}

	[RelayCommand]
	Task SaveSensitivity()
	{
		return _settingsRepository.UpdateAsync(d => d.Sensitivity = Sensitivity, CancellationToken.None);
	}


	partial void OnSensitivityChanged(int value)
	{
		// _toastable
		// 	.SetText(string.Format(Translations.MouseControl_Sensitivity, value))
		// 	.SetPosition(Position.Top)
		// 	.Show();
	}

	public async Task OnNavigatedToAsync()
	{
		var settings = await _settingsRepository.GetAsync(CancellationToken.None);
		Sensitivity = settings.Sensitivity ?? 20;

		_moveCts?.Dispose();
		_moveCts = new();
		_ = Task.Run(async() =>
		{
			while (await _streamReader.MoveNext(_moveCts.Token) && !_moveCts.IsCancellationRequested)
			{
				await _host.DesktopIntegrationClient.Desktop(d => d.SendMouseMoveAsync(_streamReader.Current.x, _streamReader.Current.y));
			}
		}, _moveCts.Token);
	}

	[RelayCommand]
	private Task SingleTap()
	{
		return _host.DesktopIntegrationClient.Desktop(d => d.SendLeftMouseClickAsync());
	}

	[RelayCommand]
	private Task DoubleTap()
	{
		return _host.DesktopIntegrationClient.Desktop(d => d.SendRightMouseClickAsync());
	}
}