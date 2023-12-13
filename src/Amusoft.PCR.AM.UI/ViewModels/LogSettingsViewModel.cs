using System.Collections.ObjectModel;
using System.ComponentModel;
using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class LogSettingsViewModel : PageViewModel, INavigationCallbacks
{
	private readonly IClientSettingsRepository _clientSettingsRepository;
	private readonly IUserInterfaceService _userInterfaceService;
	private static readonly Dictionary<string, string> FormatByDisplayFormat;

	static LogSettingsViewModel()
	{
		var refDate = new DateTime(2000, 1, 1, 12, 0, 0);
		FormatByDisplayFormat = new[] { "G", "T", "f", "g", "t", "s" }
			.Select(format => (format, display: refDate.ToString(format)))
			.ToDictionary(d => d.display, d => d.format);
	}

	public LogSettingsViewModel(ITypedNavigator navigator, IClientSettingsRepository clientSettingsRepository, IUserInterfaceService userInterfaceService) : base(navigator)
	{
		_clientSettingsRepository = clientSettingsRepository;
		_userInterfaceService = userInterfaceService;
		FormatDisplayOptions = new ObservableCollection<string>(FormatByDisplayFormat.Select(d => d.Key));
	}

	public async Task OnNavigatingAsync(INavigatingContext context)
	{
		if (ModelHasChanges)
		{
			await using var navigation = context.PauseNavigation();
			if (await _userInterfaceService.DisplayConfirmAsync(Translations.Generic_Question, Translations.Generic_SaveChangesRequest))
			{
				await SaveChangesAsync();
			}
		}
	}

	private async Task SaveChangesAsync()
	{
		if (Settings != null)
		{
			var model = Settings.ToModel();
			await _clientSettingsRepository.UpdateAsync(s =>
			{
				s.LogSettings = model;
			}, CancellationToken.None);
		}
	}

	[ObservableProperty]
	private ObservableCollection<string> _formatDisplayOptions;

	[ObservableProperty]
	private int _selectedFormatIndex;

	[ObservableProperty]
	private LogDisplaySettingsViewModel? _settings;

	private LogDisplaySettingsViewModel? _baseLine;

	[ObservableProperty]
	private bool _modelHasChanges;

	void CompareSettingsWithUnchanged(object? sender, PropertyChangedEventArgs e)
	{
		UpdateModelHasChanges();
	}

	private void UpdateModelHasChanges()
	{
		if (_baseLine is not null && Settings is not null)
		{
			ModelHasChanges = !_baseLine.Equals(Settings);
		}
	}

	protected override void OnPropertyChanged(PropertyChangedEventArgs e)
	{
		if (nameof(SelectedFormatIndex) == e.PropertyName && Settings is not null)
		{
			Settings.DateFormat = FormatByDisplayFormat[FormatDisplayOptions[SelectedFormatIndex]];
		}

		base.OnPropertyChanged(e);
	}

	public async Task OnNavigatedToAsync()
	{
		if (Settings is not null)
			Settings.PropertyChanged -= CompareSettingsWithUnchanged;

		var settings = await _clientSettingsRepository.GetAsync(CancellationToken.None);
		_baseLine = new LogDisplaySettingsViewModel(settings.LogSettings);
		Settings = new LogDisplaySettingsViewModel(settings.LogSettings);

		if (FormatByDisplayFormat.FirstOrDefault(d => d.Value.Equals(Settings.DateFormat)) is { } tuple)
		{
			SelectedFormatIndex = FormatDisplayOptions.IndexOf(tuple.Key) is var formatIndex && formatIndex >= 0 ? formatIndex : 0;
		}

		UpdateModelHasChanges();

		Settings.PropertyChanged += CompareSettingsWithUnchanged;
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.Page_Title_Settings;
	}
}