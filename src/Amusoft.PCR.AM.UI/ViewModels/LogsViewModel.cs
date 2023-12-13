using System.Collections.ObjectModel;
using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class LogsViewModel : ReloadablePageViewModel, INavigationCallbacks
{
	private readonly ILogEntryRepository _logEntryRepository;
	private readonly IClientSettingsRepository _clientSettingsRepository;
	private readonly IUserInterfaceService _userInterfaceService;

	public LogsViewModel(
		ITypedNavigator navigator
		, ILogEntryRepository logEntryRepository
		, IClientSettingsRepository clientSettingsRepository
		, IUserInterfaceService userInterfaceService) : base(navigator)
	{
		_logEntryRepository = logEntryRepository;
		_clientSettingsRepository = clientSettingsRepository;
		_userInterfaceService = userInterfaceService;
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

	[RelayCommand]
	private async Task RemoveAll()
	{
		var entryCount = await _logEntryRepository.GetCountAsync(CancellationToken.None);
		if (await _userInterfaceService.DisplayConfirmAsync(Translations.Generic_Question, string.Format(Translations.Generic_DeleteAllEntriesRequest_0, entryCount)))
			await _logEntryRepository.DeleteAllAsync(CancellationToken.None);

		await ReloadAsync();
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