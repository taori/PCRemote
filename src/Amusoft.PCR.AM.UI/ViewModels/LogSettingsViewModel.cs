using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class LogSettingsViewModel : PageViewModel, INavigationCallbacks
{
	private readonly IClientSettingsRepository _clientSettingsRepository;

	public LogSettingsViewModel(ITypedNavigator navigator, IClientSettingsRepository clientSettingsRepository) : base(navigator)
	{
		_clientSettingsRepository = clientSettingsRepository;
	}

	[ObservableProperty]
	private LogDisplaySettingsViewModel? _settings;

	public async Task OnNavigatedToAsync()
	{
		var settings = await _clientSettingsRepository.GetAsync(CancellationToken.None);
		Settings = new LogDisplaySettingsViewModel(settings.LogSettings);
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.Page_Title_Settings;
	}
}