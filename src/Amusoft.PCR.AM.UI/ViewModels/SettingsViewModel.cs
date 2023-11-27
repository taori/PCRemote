using System.Collections.ObjectModel;
using Amusoft.PCR.AM.Shared.Services;
using Amusoft.PCR.Application.Shared;
using Amusoft.PCR.Application.UI.Repos;
using Amusoft.PCR.Domain.Services;
using Amusoft.PCR.Domain.VM;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Translations = Amusoft.PCR.AM.Shared.Resources.Translations;

namespace Amusoft.PCR.Application.UI.VM;

public partial class SettingsViewModel : PageViewModel, INavigationCallbacks
{
	private readonly IUserInterfaceService _userInterfaceService;
	private readonly HostRepository _hostRepository;
	private readonly IToast _toast;

	[ObservableProperty]
	private ObservableCollection<NavigationItem<int>> _ports = new();

    protected override string GetDefaultPageTitle()
    {
        return Translations.Page_Title_Settings;
    }

    public SettingsViewModel(ITypedNavigator navigator, IUserInterfaceService userInterfaceService, HostRepository hostRepository, IToast toast) : base(navigator)
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
		await _toast.Make(AM.Shared.Resources.Translations.Generic_ChangesSaved).Show();
	}

    [RelayCommand]
    public async Task AddPort()
    {
	    if (await _userInterfaceService.GetPromptText("New Port", "Add a number", maxLength: 5) is var input && input is null)
		    return;

	    if (int.TryParse(input, out var number))
	    {
		    var addition = await _hostRepository.AddAsync(number);
		    addition.Switch(success =>
		    {
			    Ports.Add(CreateNavigationItem(number));
			    _ = _toast.Make(AM.Shared.Resources.Translations.Generic_ChangesSaved).Show();

			}, async exists =>
		    {
			    await _userInterfaceService.DisplayAlert(AM.Shared.Resources.Translations.Generic_Error, "Port already exists.");
		    });
		}
	    else
	    {
		    await _userInterfaceService.DisplayAlert(Translations.Generic_Error, "Failed to insert a number");
	    }
    }
}