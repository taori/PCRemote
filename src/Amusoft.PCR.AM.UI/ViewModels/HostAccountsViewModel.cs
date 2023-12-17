using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class HostAccountsViewModel : ReloadablePageViewModel, INavigationCallbacks
{
	private readonly IBearerTokenStorage _bearerTokenStorage;
	private readonly IHostCredentialProvider _hostCredentials;

	public HostAccountsViewModel(ITypedNavigator navigator, IBearerTokenStorage bearerTokenStorage, IHostCredentialProvider hostCredentials) : base(navigator)
	{
		_bearerTokenStorage = bearerTokenStorage;
		_hostCredentials = hostCredentials;
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.HostAccounts_Title;
	}

	public Task OnNavigatedToAsync()
	{
		return ReloadAsync();
	}

	protected override async Task OnReloadAsync(CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
		// var tokens = await _bearerTokenStorage.GetAllAsync(_hostCredentials.Address, cancellationToken);
		// // foreach (var bearerToken in tokens.OrderByDescending(d => d.Expires))
		// // {
		// // }
	}
}