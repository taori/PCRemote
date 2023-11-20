using Amusoft.PCR.Application.Resources;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.Shared;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Xml;
using Amusoft.PCR.Application.Extensions;
using Amusoft.PCR.Domain.Common;
using Amusoft.PCR.Int.IPC;
using System.Diagnostics;

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
					Text = "Spotify",
					Command = new AsyncRelayCommand(() => LaunchHostProgramAsync(_host, "spotify"))
				},
				new()
				{
					Text = "Windows Explorer",
					Command = new AsyncRelayCommand(() => LaunchHostProgramAsync(_host, "explorer"))
				},
			});
		}, _host);
	}

	private static Task LaunchHostProgramAsync(HostViewModel host, string programName, string? arguments = default)
	{
		return host.DesktopIntegrationClient?.DesktopClient.LaunchProgram(programName, arguments) ?? Task.CompletedTask;
	}
	
	[RelayCommand]
	private Task GotoKillPrograms()
	{
		return GotoKillProgramList(Navigator, _host, Translations.Programs_KillProgram, () => LoadProcessKillItems(Navigator, _host));
	}

	private static Task GotoKillProgramList(ITypedNavigator navigator, HostViewModel host, string title, Func<Task<ICollection<NavigationItem>>> source)
	{
		return navigator.OpenCommandButtonList(model =>
		{
			model.Title = title;
			model.ReloadableItemsProvider = async () => new ObservableCollection<NavigationItem>(await source());
		}, host);
	}

	private static async Task<ICollection<NavigationItem>> LoadProcessKillItems(ITypedNavigator navigator, HostViewModel host, Predicate<string>? filter = null)
	{
		if (host.DesktopIntegrationClient?.DesktopClient.GetProcessList() is {} task)
		{
			var processes = await task;
			var results = new List<NavigationItem>(processes.Value.Count);
			var appended = processes.Value.Select(item => (
					item,
					text: !string.IsNullOrEmpty(item.MainWindowTitle)
						? item.MainWindowTitle
						: !string.IsNullOrEmpty(item.ProcessName)
							? item.ProcessName
							: null,
					priority: !string.IsNullOrEmpty(item.MainWindowTitle)
						? 2
						: !string.IsNullOrEmpty(item.ProcessName)
							? 1
							: 0
				)
			);

			var rootQuery = filter == null;
			filter ??= _ => true;

			var displayList =
				rootQuery
					? appended
						.OrderByDescending(d => d.priority)
						.ThenByDescending(d => d.item.CpuUsage)
						.GroupBy(d => d.item.ProcessName, (_, tuples) => tuples.First())
					: appended
						.Where(d => d.text != null && filter(d.item.ProcessName))
						.OrderByDescending(d => d.priority)
						.ThenByDescending(d => d.item.CpuUsage);
			
			foreach (var (process, _, _) in displayList)
			{
				
				var text = !string.IsNullOrEmpty(process.MainWindowTitle)
					? process.MainWindowTitle
					: !string.IsNullOrEmpty(process.ProcessName)
						? process.ProcessName
						: null;

				if (text == null)
					continue;

				if (!string.IsNullOrEmpty(text))
				{
					if (rootQuery)
					{
						results.Add(new NavigationItem()
						{
							Text = text,
							Command = new AsyncRelayCommand(() => GotoKillProgramList(
								navigator,
								host,
								process.ProcessName,
								() => LoadProcessKillItems(navigator, host, d => d.Equals(process.ProcessName))
							))
						});
					}
					else
					{
						results.Add(new NavigationItem()
						{
							Text = $"Kill {process.ProcessId}",
							Command = new AsyncRelayCommand(() => host.DesktopIntegrationClient.DesktopClient.KillProcessById(process.ProcessId))
						});
					}
				}
			}

			return results;
		}
		else
		{
			return Array.Empty<NavigationItem>();
		}
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.Nav_Programs;
	}
}