using System.Net;
using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;

namespace Amusoft.PCR.Int.UI.ProjectDependencies;

internal class EndpointAccountSelection : IEndpointAccountSelection
{
	private readonly IClientSettingsRepository _clientSettingsRepository;
	private readonly IUserInterfaceService _userInterfaceService;

	public EndpointAccountSelection(IClientSettingsRepository clientSettingsRepository, IUserInterfaceService userInterfaceService)
	{
		_clientSettingsRepository = clientSettingsRepository;
		_userInterfaceService = userInterfaceService;
	}

	public Task<(string? mail, Guid? endpointAccountId)> GetCurrentAccountOrPromptAsync(IPEndPoint endPoint)
	{
		return GetCurrentAccountOrPromptAsync(endPoint, true);
	}

	private async Task<(string? mail, Guid? endpointAccountId)> GetCurrentAccountOrPromptAsync(IPEndPoint endPoint, bool prompt)
	{
		var settings = await _clientSettingsRepository
			.GetAsync(CancellationToken.None)
			.ConfigureAwait(false);

		if (settings.EndpointAccountIdByEndpoint.TryGetValue(endPoint, out var accountId))
		{
			return (null, accountId);
		}

		if (!prompt)
			return (null, null);

		var mail = await _userInterfaceService.GetPromptTextAsync(Translations.Generic_Question, string.Format(Translations.AccountSelection_PickEmailForEndpoint_0, endPoint));
		if (mail is null)
			return (null, null);

		return (mail, null);
	}

	public async Task<Guid?> GetCurrentAccountAsync(IPEndPoint endPoint)
	{
		var result = await GetCurrentAccountOrPromptAsync(endPoint, false);
		return result.endpointAccountId;
	}

	public async Task SetEndpointAccountAsync(IPEndPoint endPoint, Guid endpointAccountId)
	{
		await _clientSettingsRepository.UpdateAsync(d =>
		{
			d.EndpointAccountIdByEndpoint[endPoint] = endpointAccountId;
		}, CancellationToken.None);
	}
}