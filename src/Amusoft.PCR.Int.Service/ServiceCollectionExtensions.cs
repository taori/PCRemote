using Amusoft.PCR.AM.Service.Features.GettingStarted;
using Amusoft.PCR.AM.Service.Interfaces;
using Amusoft.PCR.Domain.Shared.Interfaces;
using Amusoft.PCR.Int.IPC;
using Amusoft.PCR.Int.IPC.Integration;
using Amusoft.PCR.Int.Service.Authorization;
using Amusoft.PCR.Int.Service.Interfaces;
using Amusoft.PCR.Int.Service.Repositories;
using Amusoft.PCR.Int.Service.Services;
using GrpcDotNetNamedPipes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.Int.Service;

public static class ServiceCollectionExtensions
{
	public static void AddServiceIntegration(this IServiceCollection services)
	{
		services.AddInterprocessCommunication();
		
		services.AddSingleton<IBroadcastCommunicatorFactory, BroadcastCommunicatorFactory>();
		services.AddSingleton<IImpersonatedProcessLauncher, ImpersonatedProcessLauncher>();
		services.AddSingleton<IDesktopClientMethods, DesktopServiceClientWrapper>();
		services.AddSingleton<IAgentPingService, AgentPingService>();
		services.AddSingleton<IQrCodeImageProvider, QrCodeImageProvider>();
		
		services.AddSingleton<AuthenticationStateProvider, AuthStateProvider<ApplicationUser>>();
		services.AddScoped<IAuthorizationHandler, HostCommandPermissionHandler>();
		services.AddScoped<IAuthorizationHandler, RoleOrAdminAuthorizationHandler>();
		services.AddScoped<IRoleNameProvider, DefaultRoleNameProvider>();
		services.AddScoped<IRoleNameProvider, BackendAuthorizeRoleProvider>();
		services.AddScoped<IHostCommandService, HostCommandService>();
		services.AddScoped<IUserManagementRepository, UserManagementRepository>();

		services.AddTransient<IStartupTask, ApplicationSeedTask>();
		services.AddTransient<IStartupTask, MigrationTask>();

		services.AddSingleton<DesktopIntegrationService.DesktopIntegrationServiceClient>(provider =>
		{
			return new DesktopIntegrationService.DesktopIntegrationServiceClient(provider.GetRequiredService<NamedPipeChannel>());
		});
		services.AddSingleton<NamedPipeChannel>(d => new NamedPipeChannel(".", Globals.NamedPipeChannel));
	}
}