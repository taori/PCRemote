using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Amusoft.PCR.App.Service.Services;
using Amusoft.PCR.Application;
using Amusoft.PCR.Domain.AgentSettings;

namespace Amusoft.PCR.App.Service;

public class Program
{

	public static async Task Main(string[] args)
	{
		var builder = CreateHostBuilder(args);
		
		AddServices(builder);

		RegisterAsWindowsService(builder);

		var host = builder.Build();
		ConfigureHost(host);

		await host.RunAsync();
	}

	private static WebApplicationBuilder CreateHostBuilder(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		// builder.Configuration
		// 	.SetBasePath(Directory.GetCurrentDirectory())
		// 	.AddJsonFile("appsettings.json", false, true)
		// 	.AddJsonFile("appsettings.{Environment}.json", true, true)
		// 	.AddEnvironmentVariables()
		// 	;

		return builder;
	}

	private static void ConfigureHost(WebApplication host)
	{
		host.UseRouting();

		host.UseAuthentication();
		host.UseAuthorization();

		host.MapGrpcService<PingService>();
		host.MapGrpcService<VoiceRecognitionService>();
		host.MapGrpcService<DesktopIntegrationService>();
		host.MapGrpcService<BroadcastReceiverService>();
		host.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

		if (host.Environment.IsDevelopment())
		{
			host.MapGrpcReflectionService();
		}
	}

	private static void AddServices(WebApplicationBuilder builder)
	{
		// Additional configuration is required to successfully run gRPC on macOS.
		// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

		builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));

		// Add services to the container.
		builder.Services.AddGrpc();
		builder.Services.AddGrpcReflection();

		builder.Services.AddAuthentication();
		builder.Services.AddAuthorization();
		builder.Services.AddWindowsService();

		builder.Services.AddHostedService<DesktopIntegrationLauncherServiceDelegate>();
		builder.Services.AddHostedService<ClientDiscoveryDelegate>();

		builder.Services.AddApplication();
	}

	private static void RegisterAsWindowsService(WebApplicationBuilder builder)
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			builder.Host.UseWindowsService();
	}
}