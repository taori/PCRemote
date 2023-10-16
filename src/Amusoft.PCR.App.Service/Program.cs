using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Amusoft.PCR.App.Service.Services;

namespace Amusoft.PCR.App.Service;

public class Program
{

	public static async Task Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		
		AddServices(builder);

		RegisterAsWindowsService(builder);

		var host = builder.Build();
		ConfigureHost(host);

		await host.RunAsync();
	}

	private static void ConfigureHost(WebApplication host)
	{
		host.UseRouting();

		host.UseAuthentication();
		host.UseAuthorization();

		host.MapGrpcService<PingService>();
		host.MapGrpcService<VoiceRecognitionService>();
		host.MapGrpcService<DesktopIntegrationService>();
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

		// Add services to the container.
		builder.Services.AddGrpc();
		builder.Services.AddGrpcReflection();

		builder.Services.AddAuthentication();
		builder.Services.AddAuthorization();
		builder.Services.AddWindowsService();
	}

	private static void RegisterAsWindowsService(WebApplicationBuilder builder)
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			builder.Host.UseWindowsService();
	}
}