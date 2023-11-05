using System.Collections.ObjectModel;
using Amusoft.PCR.Application.Features.DesktopIntegration;
using Amusoft.PCR.Application.Resources;
using Amusoft.PCR.Application.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.Application.UI.VM;

public partial class AudioViewModel : Shared.ReloadablePageViewModel
{
	private readonly IDesktopIntegrationServiceFactory _integrationServiceFactory;

	[ObservableProperty]
	private ObservableCollection<AudioViewModelItem>? _items;

	protected override async Task OnReloadAsync()
	{
		await Task.Delay(5000);
		Items = new ObservableCollection<AudioViewModelItem>();
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.Page_Title_Audio;
	}

	[RelayCommand]
	public Task ToggleMute()
	{
		return Task.CompletedTask;
		// return _client.Desktop(d => d.AbortShutdown()) ?? Task.CompletedTask;
	}

	public AudioViewModel(ITypedNavigator navigator, IDesktopIntegrationServiceFactory integrationServiceFactory) : base(navigator)
	{
		_integrationServiceFactory = integrationServiceFactory;
	}
}

public partial class AudioViewModelItem : ObservableObject
{

}