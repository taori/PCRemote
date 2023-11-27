using System.Collections.ObjectModel;
using Amusoft.PCR.AM.Shared.Interfaces;
using Amusoft.PCR.AM.UI.Extensions;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Translations = Amusoft.PCR.AM.Shared.Resources.Translations;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class InputControlViewModel : PageViewModel
{
	private readonly HostViewModel _host;
	private readonly IAgentEnvironment _agentEnvironment;
	private readonly IToast _toast;

	public InputControlViewModel(ITypedNavigator navigator, HostViewModel host, IAgentEnvironment agentEnvironment, IToast toast) : base(navigator)
	{
		_host = host;
		_agentEnvironment = agentEnvironment;
		_toast = toast;
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.Nav_InputControl;
	}

	[RelayCommand]
	private Task SendInput()
	{
		return Navigator.OpenCommandButtonList(model =>
		{
			model.Title = AM.Shared.Resources.Translations.InputControl_ControlOptions;
			model.Items = new ObservableCollection<NavigationItem>(new NavigationItem[]
			{
				new()
				{
					Text = AM.Shared.Resources.Translations.InputControl_ControlOptions_Windows,
					Command = ControlOptionsWindowsCommand
				},
				new()
				{
					Text = AM.Shared.Resources.Translations.InputControl_ControlOptions_VideoBrowser,
					Command = ControlOptionsBrowserVideoPlayerCommand
				},
			});
		}, _host);
	}

	[RelayCommand]
	private Task MouseControl() => 
		Navigator.ScopedNavigationAsync(d => d.AddSingleton(_host), d => d.OpenMouseControl());

	[RelayCommand]
	private Task Clipboard()
	{
		return Navigator.OpenCommandButtonList(model =>
		{
			model.Title = "Clipboard";
			model.Items = new ObservableCollection<NavigationItem>(new NavigationItem[]
			{
				new()
				{
					Text = AM.Shared.Resources.Translations.Clipboard_LoadFromHost,
					Command = RunGetClipboardCommand
				},
				new()
				{
					Text = AM.Shared.Resources.Translations.Clipboard_UpdateHost,
					Command = RunSetClipboardCommand
				},
				new()
				{
					Text = AM.Shared.Resources.Translations.Clipboard_TellCurrent,
					Command = RunTellClipboardCommand
				},
			});
		}, _host);

	}

	[RelayCommand]
	private async Task RunGetClipboard()
	{
		var task = _host.DesktopIntegrationClient?.Desktop(d => d.GetClipboardAsync(_agentEnvironment.AgentName));
		if (task != null)
		{
			var content = await task;
			if (content is { } c)
			{
				await _agentEnvironment.UpdateClipboardAsync(c);
				await _toast.Make(AM.Shared.Resources.Translations.InputControl_ClientUpdatedMessage).Show();
			}
		}
	}

	[RelayCommand]
	private async Task RunSetClipboard()
	{
		var cc = await _agentEnvironment.GetClipboardAsync();
		var updateTask = _host.DesktopIntegrationClient?.Desktop((d => d.SetClipboardAsync(_agentEnvironment.AgentName, cc ?? string.Empty)));
		if (updateTask != null)
		{
			var success = await updateTask;
			if (success == true)
				await _toast.Make(AM.Shared.Resources.Translations.InputControl_HostUpdatedMessage).Show();
		}
	}

	[RelayCommand]
	private async Task RunTellClipboard()
	{
		var content = await _agentEnvironment.GetClipboardAsync();
		if (content is { } c)
			await _toast.Make(c).Show();
	}

	[RelayCommand]
	private Task ControlOptionsWindows()
	{
		return Navigator.OpenCommandButtonList(model =>
		{
			model.Title = AM.Shared.Resources.Translations.InputControl_ControlOptions_Windows;
			model.Items = new ObservableCollection<NavigationItem>(new NavigationItem[]
			{
				new()
				{
					Text = AM.Shared.Resources.Translations.InputControl_Windows_PickAudioSource,
					Command = new AsyncRelayCommand(() => RunSendKeys("^{ESC}v"))
				},
				new()
				{
					Text = AM.Shared.Resources.Translations.InputControl_Up,
					Command = new AsyncRelayCommand(() => RunSendKeys("{UP}"))
				},
				new()
				{
					Text = AM.Shared.Resources.Translations.InputControl_Down,
					Command = new AsyncRelayCommand(() => RunSendKeys("{DOWN}"))
				},
				new()
				{
					Text = AM.Shared.Resources.Translations.InputControl_Left,
					Command = new AsyncRelayCommand(() => RunSendKeys("{LEFT}"))
				},
				new()
				{
					Text = AM.Shared.Resources.Translations.InputControl_Right,
					Command = new AsyncRelayCommand(() => RunSendKeys("{RIGHT}"))
				},
				new()
				{
					Text = AM.Shared.Resources.Translations.InputControl_Space,
					Command = new AsyncRelayCommand(() => RunSendKeys(" "))
				},
				new()
				{
					Text = AM.Shared.Resources.Translations.InputControl_Enter,
					Command = new AsyncRelayCommand(() => RunSendKeys("~"))
				},
				new()
				{
					Text = AM.Shared.Resources.Translations.InputControl_Escape,
					Command = new AsyncRelayCommand(() => RunSendKeys("{ESC}"))
				},
				new()
				{
					Text = AM.Shared.Resources.Translations.InputControl_MouseControl,
					Command = mouseControlCommand
				},
			});
		}, _host);
	}

	[RelayCommand]
	private Task ControlOptionsBrowserVideoPlayer()
	{
		return Navigator.OpenCommandButtonList(model =>
		{
			model.Title = AM.Shared.Resources.Translations.InputControl_ControlOptions_VideoBrowser;
			model.Items = new ObservableCollection<NavigationItem>(new NavigationItem[]
			{
				new()
				{
					Text = AM.Shared.Resources.Translations.InputControl_ControlOptions_VideoBrowser_PlayToggle,
					Command = new AsyncRelayCommand(() => RunSendKeys(" "))
				},
				new()
				{
					Text = AM.Shared.Resources.Translations.InputControl_Escape,
					Command = new AsyncRelayCommand(() => RunSendKeys("{ESC}"))
				},
				new()
				{
					Text = AM.Shared.Resources.Translations.InputControl_ControlOptions_Fullscreen,
					Command = new AsyncRelayCommand(() => RunSendKeys("f"))
				},
				new()
				{
					Text = AM.Shared.Resources.Translations.InputControl_ControlOptions_Forward,
					Command = new AsyncRelayCommand(() => RunSendKeys("{RIGHT}"))
				},
				new()
				{
					Text = AM.Shared.Resources.Translations.InputControl_ControlOptions_Reverse,
					Command = new AsyncRelayCommand(() => RunSendKeys("{LEFT}"))
				},
				new()
				{
					Text = AM.Shared.Resources.Translations.InputControl_ControlOptions_VolumeUp,
					Command = new AsyncRelayCommand(() => RunSendKeys("{UP}"))
				},
				new()
				{
					Text = AM.Shared.Resources.Translations.InputControl_ControlOptions_VolumeDown,
					Command = new AsyncRelayCommand(() => RunSendKeys("{DOWN}"))
				},
				new()
				{
					Text = AM.Shared.Resources.Translations.InputControl_ControlOptions_VideoBrowser_MuteToggle,
					Command = new AsyncRelayCommand(() => RunSendKeys("m"))
				},
				new()
				{
					Text = AM.Shared.Resources.Translations.InputControl_MouseControl,
					Command = mouseControlCommand
				},
			});
		}, _host);
	}

	private Task RunSendKeys(string keys)
	{
		return _host.DesktopIntegrationClient.Desktop(d => d.SendKeys(keys));
	}
}