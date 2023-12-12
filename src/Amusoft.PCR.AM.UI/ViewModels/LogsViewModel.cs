using System.Collections.ObjectModel;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using Amusoft.PCR.Domain.UI.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class LogsViewModel : ReloadablePageViewModel, INavigationCallbacks
{
	private readonly ILogEntryRepository _logEntryRepository;
	private readonly IClientSettingsRepository _clientSettingsRepository;

	public LogsViewModel(ITypedNavigator navigator, ILogEntryRepository logEntryRepository, IClientSettingsRepository clientSettingsRepository) : base(navigator)
	{
		_logEntryRepository = logEntryRepository;
		_clientSettingsRepository = clientSettingsRepository;
	}

	[ObservableProperty] private ObservableCollection<LogEntry> _items = new();

	[ObservableProperty] private LogDisplaySettingsViewModel _settings;

	public Task OnNavigatedToAsync()
	{
		return ReloadAsync();
	}

	[RelayCommand]
	private Task OpenLogSettings()
	{
		return Navigator.OpenLogSettings();
	}

	protected override string GetDefaultPageTitle()
	{
		return "Logs";
	}

	protected override async Task OnReloadAsync(CancellationToken cancellationToken)
	{
		var settings = await _clientSettingsRepository.GetAsync(cancellationToken);
		Settings = new LogDisplaySettingsViewModel(settings.LogSettings);
		Items = new ObservableCollection<LogEntry>(await Task.Run(() => _logEntryRepository.GetLogsAsync(settings.LogSettings, cancellationToken), cancellationToken));
	}
}