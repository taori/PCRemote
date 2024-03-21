﻿using Amusoft.PCR.AM.Service.Features.GettingStarted;
using Amusoft.PCR.AM.Service.Interfaces;
using Amusoft.PCR.AM.Service.Services;
using Amusoft.PCR.AM.Service.Utility;
using Amusoft.PCR.Domain.Shared.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.AM.Service.Extensions;

public static class ServiceCollectionExtensions
{
	public static void AddServiceApplicationModel(this IServiceCollection source)
	{
		source.AddScoped<ServerEndpointProvider>();
		source.AddHttpClient();
		source.AddHttpClient(HttpClientNames.Insecure).ConfigurePrimaryHttpMessageHandler(
			() => new HttpClientHandler()
			{
				ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
			}
		);
		
		source.AddSingleton<DesktopIntegrationLauncherService>();
		source.AddSingleton<ClientDiscoveryService>();
		source.AddSingleton<IIntegrationApplicationLocator, IntegrationApplicationLocator>();
		source.AddSingleton<IApplicationStateTransmitter, ApplicationStateTransmitter>();
	}
}