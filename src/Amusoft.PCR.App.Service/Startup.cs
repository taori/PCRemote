using Amusoft.PCR.App.Service.Services;

namespace Amusoft.PCR.App.Service;

public class Startup
{
	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	public IConfiguration Configuration { get; }

	// This method gets called by the runtime. Use this method to add services to the container.
	// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
	public void ConfigureServices(IServiceCollection services)
	{
		// Additional configuration is required to successfully run gRPC on macOS.
		// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

		// Add services to the container.
		services.AddGrpc();
		services.AddGrpcReflection();

		services.AddAuthentication();
		services.AddAuthorization();
		services.AddWindowsService();
	}

	public void Configure(IApplicationBuilder app,
		IWebHostEnvironment env,
		IServiceScopeFactory serviceScopeFactory,
		ILogger<Startup> logger)
	{
		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapGrpcService<PingService>();
			endpoints.MapGrpcService<VoiceRecognitionService>();
			endpoints.MapGrpcService<DesktopIntegrationService>();
			endpoints.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

			if (env.IsDevelopment())
			{
				endpoints.MapGrpcReflectionService();
			}
		});
	}
}