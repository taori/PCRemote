using Amusoft.PCR.Application.Resources;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.Shared;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Amusoft.PCR.Application.UI.VM;

public partial class ProgramsViewModel : PageViewModel
{
	private readonly HostViewModel _host;

	public ProgramsViewModel(ITypedNavigator navigator, HostViewModel host) : base(navigator)
	{
		_host = host;
	}

	[RelayCommand]
	private Task GotoStartPrograms()
	{
		return Navigator.OpenCommandButtonList(model =>
		{
			model.Title = Translations.Programs_StartProgram;
			model.Items = new ObservableCollection<NavigationItem>(new NavigationItem[]
			{
				new()
				{
					Text = Translations.InputControl_ControlOptions_VideoBrowser_PlayToggle,
					Command = new AsyncRelayCommand(() => Task.CompletedTask)
				},
			});
		}, _host);
	}

	[RelayCommand]
	private Task GotoKillPrograms()
	{
		return Navigator.OpenCommandButtonList(model =>
		{
			model.Title = Translations.Programs_KillProgram;
			model.Items = new ObservableCollection<NavigationItem>(new NavigationItem[]
			{
				new()
				{
					Text = Translations.InputControl_ControlOptions_VideoBrowser_PlayToggle,
					Command = new AsyncRelayCommand(() => Task.CompletedTask)
				},
			});
		}, _host);
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.Nav_Programs;
	}
}