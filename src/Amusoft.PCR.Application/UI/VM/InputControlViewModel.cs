using System.Collections.ObjectModel;
using Amusoft.PCR.Application.Extensions;
using Amusoft.PCR.Application.Resources;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.Shared;
using Amusoft.PCR.Domain.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.Application.UI.VM;

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
			model.Title = Translations.InputControl_ControlOptions;
			model.Items = new ObservableCollection<NavigationItem>(new NavigationItem[]
			{
				new()
				{
					Text = Translations.InputControl_ControlOptions_Windows,
					Command = ControlOptionsWindowsCommand
				},
				new()
				{
					Text = Translations.InputControl_ControlOptions_VideoBrowser,
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
					Text = Translations.Clipboard_LoadFromHost,
					Command = RunGetClipboardCommand
				},
				new()
				{
					Text = Translations.Clipboard_UpdateHost,
					Command = RunSetClipboardCommand
				},
				new()
				{
					Text = Translations.Clipboard_TellCurrent,
					Command = RunTellClipboardCommand
				},
			});
		}, _host);

	}

	[RelayCommand]
	private async Task RunGetClipboard()
	{
		var content = await _host.DesktopIntegrationClient?.Desktop(d => d.GetClipboardAsync(_agentEnvironment.AgentName));
		if (content is { } c)
		{
			await _agentEnvironment.UpdateClipboardAsync(c);
			await _toast.Make(Translations.InputControl_ClientUpdatedMessage).Show();
		}
	}

	[RelayCommand]
	private async Task RunSetClipboard()
	{
		var cc = await _agentEnvironment.GetClipboardAsync();
		var success = await _host.DesktopIntegrationClient?.Desktop((d => d.SetClipboardAsync(_agentEnvironment.AgentName, cc)));
		if (success == true)
			await _toast.Make(Translations.InputControl_HostUpdatedMessage).Show();
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
			model.Title = Translations.InputControl_ControlOptions_Windows;
			model.Items = new ObservableCollection<NavigationItem>(new NavigationItem[]
			{
				new()
				{
					Text = Translations.InputControl_Windows_PickAudioSource,
					Command = new AsyncRelayCommand(() => RunSendKeys("^{ESC}v"))
				},
				new()
				{
					Text = Translations.InputControl_Up,
					Command = new AsyncRelayCommand(() => RunSendKeys("{UP}"))
				},
				new()
				{
					Text = Translations.InputControl_Down,
					Command = new AsyncRelayCommand(() => RunSendKeys("{DOWN}"))
				},
				new()
				{
					Text = Translations.InputControl_Left,
					Command = new AsyncRelayCommand(() => RunSendKeys("{LEFT}"))
				},
				new()
				{
					Text = Translations.InputControl_Right,
					Command = new AsyncRelayCommand(() => RunSendKeys("{RIGHT}"))
				},
				new()
				{
					Text = Translations.InputControl_Space,
					Command = new AsyncRelayCommand(() => RunSendKeys(" "))
				},
				new()
				{
					Text = Translations.InputControl_Enter,
					Command = new AsyncRelayCommand(() => RunSendKeys("~"))
				},
				new()
				{
					Text = Translations.InputControl_Escape,
					Command = new AsyncRelayCommand(() => RunSendKeys("{ESC}"))
				},
				new()
				{
					Text = Translations.InputControl_MouseControl,
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
			model.Title = Translations.InputControl_ControlOptions_VideoBrowser;
			model.Items = new ObservableCollection<NavigationItem>(new NavigationItem[]
			{
				new()
				{
					Text = Translations.InputControl_ControlOptions_VideoBrowser_PlayToggle,
					Command = new AsyncRelayCommand(() => RunSendKeys(" "))
				},
				new()
				{
					Text = Translations.InputControl_Escape,
					Command = new AsyncRelayCommand(() => RunSendKeys("{ESC}"))
				},
				new()
				{
					Text = Translations.InputControl_ControlOptions_Fullscreen,
					Command = new AsyncRelayCommand(() => RunSendKeys("f"))
				},
				new()
				{
					Text = Translations.InputControl_ControlOptions_Forward,
					Command = new AsyncRelayCommand(() => RunSendKeys("{RIGHT}"))
				},
				new()
				{
					Text = Translations.InputControl_ControlOptions_Reverse,
					Command = new AsyncRelayCommand(() => RunSendKeys("{LEFT}"))
				},
				new()
				{
					Text = Translations.InputControl_ControlOptions_VolumeUp,
					Command = new AsyncRelayCommand(() => RunSendKeys("{UP}"))
				},
				new()
				{
					Text = Translations.InputControl_ControlOptions_VolumeDown,
					Command = new AsyncRelayCommand(() => RunSendKeys("{DOWN}"))
				},
				new()
				{
					Text = Translations.InputControl_ControlOptions_VideoBrowser_MuteToggle,
					Command = new AsyncRelayCommand(() => RunSendKeys("m"))
				},
				new()
				{
					Text = Translations.InputControl_MouseControl,
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