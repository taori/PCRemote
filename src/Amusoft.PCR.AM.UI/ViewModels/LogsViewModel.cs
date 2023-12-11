using System.Collections.ObjectModel;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using Amusoft.PCR.Domain.UI.Entities;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class LogsViewModel : ReloadablePageViewModel, INavigationCallbacks
{
	private readonly ILogEntryRepository _logEntryRepository;

	public LogsViewModel(ITypedNavigator navigator, ILogEntryRepository logEntryRepository) : base(navigator)
	{
		_logEntryRepository = logEntryRepository;
	}

	[ObservableProperty] private ObservableCollection<LogEntry> _items = new();

	public Task OnNavigatedToAsync()
	{
		return ReloadAsync();
	}

	protected override string GetDefaultPageTitle()
	{
		return "Logs";
	}

	protected override async Task OnReloadAsync(CancellationToken cancellationToken)
	{
		Items = new ObservableCollection<LogEntry>(await Task.Run(() => _logEntryRepository.GetLogsSinceAsync(DateTime.Now.AddDays(-1), cancellationToken), cancellationToken));
		;
	}
}