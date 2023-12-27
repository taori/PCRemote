using System.Net;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.Domain.UI.Entities;

namespace Amusoft.PCR.Int.UI.ProjectDependencies;

internal class EndpointAccountManager : IEndpointAccountManager
{
	private readonly IEndpointAccountSelection _endpointAccountSelection;
	private readonly IEndpointRepository _endpointRepository;
	private readonly IBearerTokenRepository _bearerTokenRepository;

	public EndpointAccountManager(IEndpointAccountSelection endpointAccountSelection, IEndpointRepository endpointRepository, IBearerTokenRepository bearerTokenRepository)
	{
		_endpointAccountSelection = endpointAccountSelection;
		_endpointRepository = endpointRepository;
		_bearerTokenRepository = bearerTokenRepository;
	}

	public async Task<EndpointAccount?> TryGetEndpointAccountAsync(IPEndPoint endPoint, CancellationToken cancellationToken)
	{
		if (await _endpointAccountSelection.GetCurrentAccountOrPromptAsync(endPoint, cancellationToken) is var currentSelection && currentSelection is (null, null))
			return default;

		var (mail, currentId) = currentSelection;
		if (currentId is not null)
			return await _endpointRepository.GetEndpointAccountAsync(currentId.Value, cancellationToken);

		if (mail is null)
			return default;

		if (await _endpointRepository.FindEndpointAsync(endPoint, cancellationToken) is var endpointDto && endpointDto is null)
		{
			endpointDto = await _endpointRepository.CreateEndpointAsync(endPoint, cancellationToken);
		}

		if (await _endpointRepository.FindEndpointAccountAsync(endpointDto.Id, mail, cancellationToken) is var endpointAccountDto && endpointAccountDto is null)
		{
			endpointAccountDto = await _endpointRepository.CreateEndpointAccountAsync(endpointDto.Id, mail, cancellationToken);
			await _endpointAccountSelection.SetEndpointAccountAsync(endPoint, endpointAccountDto.Id, cancellationToken);
		}

		return endpointAccountDto;
	}

	public Task AddBearerTokenAsync(BearerToken bearerToken)
	{
		return _bearerTokenRepository.AddTokenAsync(bearerToken, CancellationToken.None);
	}
}