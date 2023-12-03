using System.Numerics;
using System.Threading.Channels;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Translations = Amusoft.PCR.AM.Shared.Resources.Translations;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class MouseControlViewModel : PageViewModel, INavigationCallbacks
{
	private readonly HostViewModel _host;
	private readonly IClientSettingsRepository _settingsRepository;
	private readonly IToast _toast;

	private CancellationTokenSource? _moveCts = new();

	private readonly Channel<(int x, int y)> _mouseMoveChannel;

	public MouseControlViewModel(ITypedNavigator navigator, HostViewModel host, IClientSettingsRepository settingsRepository, IToast toast) : base(navigator)
	{
		_host = host;
		_settingsRepository = settingsRepository;
		_toast = toast;
		_mouseMoveChannel = Channel.CreateUnbounded<(int x, int y)>();
		_toastable = _toast.Make("").SetPosition(Position.Center);
	}

	protected override string GetDefaultPageTitle()
	{
		return AM.Shared.Resources.Translations.InputControl_MouseControl;
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
	private async Task SaveSensitivity()
	{
		await _settingsRepository.UpdateAsync(d => d.Sensitivity = Sensitivity, CancellationToken.None);
		await _toast
			.Make(AM.Shared.Resources.Translations.Generic_ChangesSaved)
			.SetPosition(Position.Bottom)
			.Show();
	}


	partial void OnSensitivityChanged(int value)
	{
		_toastable
			.SetText(string.Format(Translations.MouseControl_Sensitivity, value))
			.Show();
	}

	public async Task OnNavigatedToAsync()
	{
		var settings = await _settingsRepository.GetAsync(CancellationToken.None);
		Sensitivity = settings.Sensitivity ?? 20;

		_moveCts?.Dispose();
		_moveCts = new();
		_ = Task.Run(async() =>
		{
			while (await _mouseMoveChannel.Reader.WaitToReadAsync(_moveCts.Token) && !_moveCts.IsCancellationRequested)
			{
				var tuple = await _mouseMoveChannel.Reader.ReadAsync(_moveCts.Token);
				await _host.IpcClient.DesktopClient.SendMouseMoveAsync(tuple.x, tuple.y);
			}
		}, _moveCts.Token);
	}

	[RelayCommand]
	private Task SingleTap()
	{
		return _host.IpcClient.DesktopClient.SendLeftMouseClickAsync();
	}

	[RelayCommand]
	private Task DoubleTap()
	{
		return _host.IpcClient.DesktopClient.SendRightMouseClickAsync();
	}
}