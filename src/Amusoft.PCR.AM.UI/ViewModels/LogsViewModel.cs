using System.Collections.ObjectModel;
using System.ComponentModel;
using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class LogsViewModel(
	ITypedNavigator navigator
	, ILogger<LogsViewModel> logger
	, ILogEntryRepository logEntryRepository
	, IClientSettingsRepository clientSettingsRepository
	, IUserInterfaceService userInterfaceService
)
	: ReloadablePageViewModel(navigator), INavigationCallbacks
{
	private readonly ILogger<LogsViewModel> _logger = logger;

	[ObservableProperty]
	private ObservableCollection<LogEntryViewModel> _items = new();

	[ObservableProperty]
	private LogDisplaySettingsViewModel? _settings;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PaginationText))]
	private int _currentPage;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PaginationText))]
	private int _maxPage;

	public string PaginationText => string.Format(Translations.Generic_Pagination_0_1, CurrentPage + 1, MaxPage + 1);

	public Task OnNavigatedToAsync()
	{
		CurrentPage = 0;
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
		var entryCount = await logEntryRepository.GetCountAsync(null, CancellationToken.None);
		if (await userInterfaceService.DisplayConfirmAsync(Translations.Generic_Question, string.Format(Translations.Generic_DeleteAllEntriesRequest_0, entryCount)))
			await logEntryRepository.DeleteAllAsync(CancellationToken.None);

		await ReloadAsync();
	}

	protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
	{
		try
		{
			if (nameof(CurrentPage).Equals(e.PropertyName))
			{
				await ReloadAsync();
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, nameof(OnPropertyChanged));
		}

		base.OnPropertyChanged(e);
	}

	protected override string GetDefaultPageTitle()
	{
		return "Logs";
	}

	protected override async Task OnReloadAsync(CancellationToken cancellationToken)
	{
		var settings = await clientSettingsRepository.GetAsync(cancellationToken);
		Settings = new LogDisplaySettingsViewModel(settings.LogSettings);
		var count = await logEntryRepository.GetCountAsync(settings.LogSettings, CancellationToken.None);
		MaxPage = count / settings.LogSettings.EntriesPerPage;

		var vmItems = await Task.Run(async () =>
		{
			var entries = await logEntryRepository
				.GetLogsAsync(CurrentPage, settings.LogSettings, cancellationToken)
				.ConfigureAwait(false);
			return entries
				.Select(d => new LogEntryViewModel(d, settings.LogSettings))
				.ToList();
		}, cancellationToken);

		Items = new ObservableCollection<LogEntryViewModel>(vmItems);
	}
}