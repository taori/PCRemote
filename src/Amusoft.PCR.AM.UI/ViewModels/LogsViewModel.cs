using System.Collections.ObjectModel;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using Amusoft.PCR.Domain.UI.Entities;
using Amusoft.PCR.Domain.UI.ValueTypes;
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

	[ObservableProperty]
	private ObservableCollection<LogEntryViewModel> _items = new();

	[ObservableProperty]
	private LogDisplaySettingsViewModel _settings;

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

		var vmItems = await Task.Run(async () =>
		{
			var entries = await _logEntryRepository.GetLogsAsync(settings.LogSettings, cancellationToken).ConfigureAwait(false);
			return entries
				.Select(d => new LogEntryViewModel(d, settings.LogSettings))
				.ToList();
		}, cancellationToken);

		Items = new ObservableCollection<LogEntryViewModel>(vmItems);
	}
}

public class LogEntryViewModel
{
	private readonly LogEntry _item;
	private readonly LogSettings _settings;

	public LogEntryViewModel(LogEntry item, LogSettings settings)
	{
		_item = item;
		_settings = settings;
	}

	public string Time => _item.Time.ToString(_settings.DateFormat);
	public string Logger => _item.Logger;
	public string Message => _item.Message;
	public LogEntryType LogLevel => _item.LogLevel;
}