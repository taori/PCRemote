using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using Amusoft.PCR.Domain.UI.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class HostAccountCreationViewModel : PageViewModel, INavigationCallbacks
{
	private readonly Endpoint _endpoint;
	private readonly IHostCredentials _hostCredentials;

	[ObservableProperty]
	private string _hostAccountDisplayName;

	public HostAccountCreationViewModel(ITypedNavigator navigator, Endpoint endpoint, IHostCredentials hostCredentials) : base(navigator)
	{
		_endpoint = endpoint;
		_hostCredentials = hostCredentials;
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.HostAccountCreation_Title;
	}

	[RelayCommand]
	private Task CompleteCreation()
	{
		return Task.CompletedTask;
	}

	public Task OnNavigatedToAsync()
	{
		HostAccountDisplayName = _hostCredentials.Title;
		return Task.CompletedTask;
	}
}