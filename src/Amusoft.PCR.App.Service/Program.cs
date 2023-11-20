using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Amusoft.PCR.App.Service.Services;
using Amusoft.PCR.Application;
using Amusoft.PCR.Application.Features.DesktopIntegration;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Domain.AgentSettings;
using Amusoft.PCR.Domain.Services;
using Amusoft.PCR.Int.IPC;
using GrpcDotNetNamedPipes;
using Microsoft.AspNetCore.StaticFiles;
using NLog;
using NLog.Fluent;
using NLog.Web;
using DesktopIntegrationService = Amusoft.PCR.App.Service.Services.DesktopIntegrationService;

namespace Amusoft.PCR.App.Service;

public class Program
{
	private static Logger? _logger;

	public static async Task Main(string[] args)
	{
		_logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();

		TaskScheduler.UnobservedTaskException += (sender, ex) => _logger.Fatal(ex);
		AppDomain.CurrentDomain.UnhandledException += (sender, ex) => _logger.Fatal(ex);

		try
		{
			var builder = CreateHostBuilder(args);
			builder.Host.UseNLog();

			AddServices(builder);

			RegisterAsWindowsService(builder);

			var host = builder.Build();
			ConfigureHost(host);

			await host.RunAsync();
		}
		catch (Exception e)
		{
			_logger.Error(e, "Stopped program because of exception");
		}
		finally
		{
			// Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
			LogManager.Flush();
			LogManager.Shutdown();
		}
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
		var logger = host.Services.GetRequiredService<ILogger<Program>>();
		var applicationStateTransmitter = host.Services.GetRequiredService<IApplicationStateTransmitter>();
		applicationStateTransmitter.NotifyConfigurationDone();

		host.UseRouting();
		host.UseGrpcWeb(new GrpcWebOptions() {DefaultEnabled = true});

		host.UseAuthentication();
		host.UseAuthorization();

		host.MapGrpcService<PingService>();
		host.MapGrpcService<VoiceRecognitionService>();
		host.MapGrpcService<DesktopIntegrationService>();
		
#if DEBUG
		host.MapGet("/download/test", (context) => context.RequestServices.GetRequiredService<IWwwFileLoader>().GetTestFile().ExecuteAsync(context));
#endif

		host.MapGet("/download/android", (context) => context.RequestServices.GetRequiredService<IWwwFileLoader>().GetAndroidApp().ExecuteAsync(context));


		host.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

		if (host.Environment.IsDevelopment())
		{
			logger.LogWarning("GRPC Reflection service is enabled");
			host.MapGrpcReflectionService();
		}
	}

	private static void AddServices(WebApplicationBuilder builder)
	{
		// Additional configuration is required to successfully run gRPC on macOS.
		// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

		builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));

		// Add services to the container.
		builder.Services.AddLogging();
		builder.Services.AddGrpc();
		builder.Services.AddGrpcReflection();

		builder.Services.AddAuthentication();
		builder.Services.AddAuthorization();
		builder.Services.AddWindowsService();

		builder.Services.AddHostedService<DesktopIntegrationLauncherServiceDelegate>();
		builder.Services.AddHostedService<ClientDiscoveryDelegate>();
		
		builder.Services.AddSingleton<IWwwFileLoader, WwwFileLoader>();
		builder.Services.AddSingleton<IConnectedServerPorts, ConnectedServerPorts>();
		builder.Services.AddSingleton<Int.IPC.DesktopIntegrationService.DesktopIntegrationServiceClient>(provider =>
		{
			return new Int.IPC.DesktopIntegrationService.DesktopIntegrationServiceClient(provider.GetRequiredService<NamedPipeChannel>());
		});
		builder.Services.AddSingleton<IDesktopClientMethods, DesktopServiceClientWrapper>();
		builder.Services.AddSingleton<NamedPipeChannel>(d => new NamedPipeChannel(".", Globals.NamedPipeChannel));

		builder.Services.AddApplication();
	}

	private static void RegisterAsWindowsService(WebApplicationBuilder builder)
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			builder.Host.UseWindowsService();
	}
}