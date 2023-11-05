using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.Application.Services;

public interface INestedServiceProviderFactory
{
	IServiceProvider FromCurrentScope(Action<IServiceCollection> configuration);
}