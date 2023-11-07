using System.Collections.ObjectModel;
using Amusoft.PCR.Application.Resources;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.Shared;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.Application.UI.VM;

public partial class InputControlViewModel : PageViewModel
{
	private readonly HostViewModel _host;

	public InputControlViewModel(ITypedNavigator navigator, HostViewModel host) : base(navigator)
	{
		_host = host;
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
			model.Title = "Control Options";
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
		});
	}

	[RelayCommand]
	private Task MouseControl() => 
		Navigator.ScopedNavigationAsync(d => d.AddSingleton(_host), d => d.OpenMouseControl());

	[RelayCommand]
	private Task Clipboard(){ return Task.CompletedTask;}
}