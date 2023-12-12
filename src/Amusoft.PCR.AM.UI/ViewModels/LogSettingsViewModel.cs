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

	public LogSettingsViewModel(ITypedNavigator navigator, IClientSettingsRepository clientSettingsRepository, IUserInterfaceService userInterfaceService) : base(navigator)
	{
		_clientSettingsRepository = clientSettingsRepository;
		_userInterfaceService = userInterfaceService;
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

	public async Task OnNavigatedToAsync()
	{
		if (Settings is not null)
			Settings.PropertyChanged -= CompareSettingsWithUnchanged;

		var settings = await _clientSettingsRepository.GetAsync(CancellationToken.None);
		_baseLine = new LogDisplaySettingsViewModel(settings.LogSettings);
		Settings = new LogDisplaySettingsViewModel(settings.LogSettings);

		UpdateModelHasChanges();

		Settings.PropertyChanged += CompareSettingsWithUnchanged;
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.Page_Title_Settings;
	}
}