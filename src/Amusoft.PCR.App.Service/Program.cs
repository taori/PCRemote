using Amusoft.PCR.App.Service.Services;

namespace Amusoft.PCR.App.Service;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Additional configuration is required to successfully run gRPC on macOS.
		// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

		// Add services to the container.
		builder.Services.AddGrpc();
		builder.Services.AddGrpcReflection();

		builder.Services.AddAuthentication();
		builder.Services.AddAuthorization();

		var app = builder.Build();

		app.UseRouting();

		app.UseAuthentication();
		app.UseAuthorization();
		
		app.MapGrpcService<PingService>();
		app.MapGrpcService<VoiceRecognitionService>();
		app.MapGrpcService<DesktopIntegrationService>();
		app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
		
		if (app.Environment.IsDevelopment())
		{
			app.MapGrpcReflectionService();
		}

		app.Run();
	}
}