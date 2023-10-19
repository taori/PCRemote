using Amusoft.PCR.Application.Features.DesktopIntegration;
using Amusoft.PCR.Application.Utility;
using Amusoft.PCR.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.Application;

public static class ServiceCollectionExtensions
{
	public static void AddApplication(this IServiceCollection source)
	{
		source.AddSingleton<DesktopIntegrationLauncherService>();
		source.AddSingleton<ClientDiscoveryService>();
		source.AddSingleton<IIntegrationApplicationLocator, IntegrationApplicationLocator>();
		source.AddSingleton<IApplicationStateTransmitter, ApplicationStateTransmitter>();
	}
}