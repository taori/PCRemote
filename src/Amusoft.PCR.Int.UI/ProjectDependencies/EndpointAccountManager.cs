using System.Net;
using Amusoft.PCR.AM.UI.Interfaces;

namespace Amusoft.PCR.Int.UI.ProjectDependencies;

internal class EndpointAccountManager : IEndpointAccountManager
{
	private readonly IEndpointAccountSelection _endpointAccountSelection;
	private readonly IEndpointRepository _endpointRepository;

	public EndpointAccountManager(IEndpointAccountSelection endpointAccountSelection, IEndpointRepository endpointRepository)
	{
		_endpointAccountSelection = endpointAccountSelection;
		_endpointRepository = endpointRepository;
	}

	public async Task<Guid?> GetEndpointAccountIdAsync(IPEndPoint endPoint)
	{
		if (await _endpointAccountSelection.GetCurrentAccountOrPromptAsync(endPoint) is var currentSelection && currentSelection is (null, null))
			return default;

		var (mail, currentId) = currentSelection;
		if (currentId is not null)
			return currentId;

		if (mail is null)
			return default;

		if (await _endpointRepository.GetEndpointIdAsync(endPoint) is var endPointId && endPointId is null)
		{
			endPointId = await _endpointRepository.CreateEndpointAsync(endPoint);
		}

		if (await _endpointRepository.GetEndpointAccountIdAsync(endPointId.Value, mail) is var accountId && accountId is null)
		{
			var endpointAccountId = await _endpointRepository.CreateEndpointAccountAsync(endPointId.Value, mail);
			await _endpointAccountSelection.SetEndpointAccountAsync(endPoint, endpointAccountId);
			return endpointAccountId;
		}

		return null;
	}
}