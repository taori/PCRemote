using System.Collections.ObjectModel;
using Amusoft.PCR.Application.Resources;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.Shared;
using Amusoft.PCR.Application.UI.Repos;
using Amusoft.PCR.Domain.Services;
using Amusoft.PCR.Domain.VM;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.Application.UI.VM;

public partial class SettingsViewModel : PageViewModel, INavigationCallbacks
{
	private readonly IUserInterfaceService _userInterfaceService;
	private readonly HostRepository _hostRepository;

	[ObservableProperty]
	private ObservableCollection<NavigationItem<int>> _ports = new();

    protected override string GetDefaultPageTitle()
    {
        return Resources.Translations.Page_Title_Settings;
    }

    public SettingsViewModel(ITypedNavigator navigator, IUserInterfaceService userInterfaceService, HostRepository hostRepository) : base(navigator)
    {
	    _userInterfaceService = userInterfaceService;
	    _hostRepository = hostRepository;
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

		    }, async exists =>
		    {
			    await _userInterfaceService.DisplayAlert(Translations.Generic_Error, "Port already exists.");
		    });
		}
	    else
	    {
		    await _userInterfaceService.DisplayAlert(Translations.Generic_Error, "Failed to insert a number");
	    }
    }
}