using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.AM.Shared.Interfaces;

public interface INestedServiceProviderFactory
{
	IServiceProvider FromCurrentScope(Action<IServiceCollection> configuration);
}