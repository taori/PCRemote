using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Amusoft.PCR.Application.Extensions;

public static class ServiceProviderExtensions
{
	public static ServiceCollection BuildServiceCollectionFromProvider(this IServiceProvider serviceProvider)
	{
		var serviceCollection = new ServiceCollection();

		var descriptors = serviceProvider.GetService<IEnumerable<ServiceDescriptor>>();
		if (descriptors is not null)
		{
			foreach (var descriptor in descriptors)
			{
				serviceCollection.Add(descriptor);
			}
		}
		
		// var sp = new Service
		// // serviceProvider.GetServices()
		// var services = serviceProvider.GetServices<object>();
		//
		// foreach (var service in services)
		// {
		// 	serviceCollection.Add(service.GetType());
		// }

		return serviceCollection;
	}
}