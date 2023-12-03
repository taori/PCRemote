using Amusoft.PCR.Domain.Shared.Interfaces;
using Amusoft.PCR.Int.IPC.Integration;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.Int.IPC;

public static class ServiceCollectionExtensions
{
	public static void AddInterprocessCommunication(this IServiceCollection services)
	{
		services.AddSingleton<IDiscoveryMessageInterface, DiscoveryMessageInterface>();
	}
}