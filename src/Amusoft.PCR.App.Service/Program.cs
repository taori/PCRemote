using System.Runtime.InteropServices;
using Amusoft.PCR.App.Service.Services;

namespace Amusoft.PCR.App.Service;

public class Program
{
	public static IHostBuilder CreateHostBuilder(string[] args) =>
		Host.CreateDefaultBuilder(args)
			.ConfigureLogging(logging =>
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					logging.AddEventLog(settings =>
					{
						settings.Filter = (s, level) => level >= LogLevel.Information;
					});
				}
			})
			.UseWindowsService()
			.ConfigureServices(services =>
			{
				// services.AddHostedService<DesktopIntegrationLauncherService>();
				// services.AddHostedService<ClientDiscoveryService>();
				// services.AddHostedService<SeedService>();
				// services.AddHostedService<DbCleanupService>();
				// services.AddHostedService<VoiceRecognitionUpdateService>();
			})
			.ConfigureWebHostDefaults(webBuilder =>
			{
				webBuilder.UseStartup<Startup>();
			});

	public static async Task Main(string[] args)
	{
		await CreateHostBuilder(args).Build().RunAsync();
	}
}