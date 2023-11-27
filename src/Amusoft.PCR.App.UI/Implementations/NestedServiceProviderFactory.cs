using System.Reflection;
using Amusoft.PCR.AM.Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Amusoft.PCR.App.UI.Implementations;

public class NestedServiceProviderFactory : INestedServiceProviderFactory
{
	private readonly IServiceScopeFactory _serviceScopeFactory;

	public NestedServiceProviderFactory(IServiceScopeFactory serviceScopeFactory)
	{
		_serviceScopeFactory = serviceScopeFactory;
	}

	public IServiceProvider FromCurrentScope(Action<IServiceCollection> configuration)
	{
		var serviceCollection = new ServiceCollection();
		using var scope = _serviceScopeFactory.CreateScope();

		var descriptors = GetDescriptors(scope.ServiceProvider);
		foreach (var descriptor in descriptors)
		{
			serviceCollection.Add(descriptor);
		}

		configuration(serviceCollection);

		return serviceCollection.BuildServiceProvider(true);
	}

	private IEnumerable<ServiceDescriptor> GetDescriptors(IServiceProvider serviceProvider)
	{
		var originType = serviceProvider.GetType();
		if (originType.Name.Equals("ServiceProviderEngineScope"))
		{
			if (originType.GetProperty("RootProvider", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(serviceProvider) is IServiceProvider sp)
			{
				foreach (var serviceDescriptor in GetDescriptors(sp))
				{
					yield return serviceDescriptor;
				}
			}

			yield break;
		}
		
		var callSiteFactory = serviceProvider?.GetType().GetProperty("CallSiteFactory", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(serviceProvider);
		var descriptors = callSiteFactory?.GetType().GetField("_descriptors", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(callSiteFactory);
		if (descriptors is IEnumerable<ServiceDescriptor> refDescriptors)
		{
			foreach (var serviceDescriptor in refDescriptors)
			{
				yield return serviceDescriptor;
			}
		}
	}
}