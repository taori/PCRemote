using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.AM.Shared.Services;

public interface INestedServiceProviderFactory
{
	IServiceProvider FromCurrentScope(Action<IServiceCollection> configuration);
}