namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IEndpointTokenBrokerFactory
{
	Task<IEndpointTokenBroker> CreateAsync(Guid endpointAccountId);
}