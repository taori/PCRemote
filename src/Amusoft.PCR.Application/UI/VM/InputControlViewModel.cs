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
		return Navigator.OpenStaticCommandButtonList(model =>
		{
			model.Title = "Control options";
			model.Items = new ObservableCollection<NavigationItem>(new NavigationItem[]
			{
				new()
				{
					Text = "Windows",
					Command = new AsyncRelayCommand(() => _host.DesktopIntegrationClient?.DesktopClient.SendKeys("^{ESC}v") ?? Task.CompletedTask)
				},
				new()
				{
					Text = "Task Manager",
					Command = new AsyncRelayCommand(() => _host.DesktopIntegrationClient?.DesktopClient.SendKeys("^%{DEL}") ?? Task.CompletedTask)
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
		return Navigator.OpenStaticCommandButtonList(model =>
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
			await _toast.Make("Client clipboard updated").Show();
		}
	}

	[RelayCommand]
	private async Task RunSetClipboard()
	{
		var cc = await _agentEnvironment.GetClipboardAsync();
		var success = await _host.DesktopIntegrationClient?.Desktop((d => d.SetClipboardAsync(_agentEnvironment.AgentName, cc)));
		if (success == true)
			await _toast.Make("Host clipboard updated").Show();
	}

	[RelayCommand]
	private async Task RunTellClipboard()
	{
		var content = await _agentEnvironment.GetClipboardAsync();
		if (content is { } c)
			await _toast.Make(c).Show();
	}
}