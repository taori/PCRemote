using System.Collections.ObjectModel;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using Amusoft.PCR.Domain.UI.ValueTypes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Translations = Amusoft.PCR.AM.Shared.Resources.Translations;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class SettingsViewModel : PageViewModel, INavigationCallbacks
{
	private readonly IUserInterfaceService _userInterfaceService;
	private readonly IHostRepository _hostRepository;
	private readonly IToast _toast;

	[ObservableProperty]
	private ObservableCollection<NavigationItem<int>> _ports = new();

    protected override string GetDefaultPageTitle()
    {
        return Translations.Page_Title_Settings;
    }

    public SettingsViewModel(ITypedNavigator navigator, IUserInterfaceService userInterfaceService, IHostRepository hostRepository, IToast toast) : base(navigator)
    {
	    _userInterfaceService = userInterfaceService;
	    _hostRepository = hostRepository;
	    _toast = toast;
    }

    public async Task OnNavigatedToAsync()
    {
	    var ports = await _hostRepository.GetHostPortsAsync();
		Ports = new ObservableCollection<NavigationItem<int>>(ports.Select(CreateNavigationItem));
    }

    private NavigationItem<int> CreateNavigationItem(int port)
    {
	    return new NavigationItem<int>(port)
	    {
		    Command = RemovePortCommand,
		    Text = port.ToString()
	    };
    }

    [RelayCommand]
    public async Task RemovePort(NavigationItem<int> portItem)
	{
		await _hostRepository.RemoveAsync(portItem.Value);
		Ports.Remove(portItem);
		await _toast.Make(Translations.Generic_ChangesSaved).Show();
	}

    [RelayCommand]
    public async Task AddPort()
    {
	    if (await _userInterfaceService.GetPromptText(Translations.Settings_NewPort, Translations.Settings_AddNumber, maxLength: 5, keyboard: Keyboard.Numeric) is var input && input is null)
		    return;

	    if (int.TryParse(input, out var number))
	    {
		    var addition = await _hostRepository.AddAsync(number);
		    addition.Switch(success =>
		    {
			    Ports.Add(CreateNavigationItem(number));
			    _ = _toast.Make(Translations.Generic_ChangesSaved).Show();

			}, async exists =>
		    {
			    await _userInterfaceService.DisplayAlert(Translations.Generic_Error, Translations.Settings_PortAlreadyExists);
		    });
		}
	    else
	    {
		    await _userInterfaceService.DisplayAlert(Translations.Generic_Error, Translations.Settings_FailedToInsertPort);
	    }
    }
}