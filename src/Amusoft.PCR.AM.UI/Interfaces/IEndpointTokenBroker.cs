namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IEndpointTokenBroker
{
	Task<string?> GetAccessTokenAsync();
}