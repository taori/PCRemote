using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Amusoft.PCR.AM.Service.Extensions;
using Amusoft.PCR.AM.Service.Interfaces;
using Amusoft.PCR.AM.Shared.Interfaces;
using Amusoft.PCR.App.Service.HealthChecks;
using Amusoft.PCR.App.Service.Services;
using Amusoft.PCR.Domain.Service.Entities;
using Amusoft.PCR.Int.IPC;
using Amusoft.PCR.Int.Service;
using Amusoft.PCR.Int.Service.Authorization;
using Amusoft.PCR.Int.Service.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;

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
		// 	.AddEnvironmentVariables();

		return builder;
	}

	private static void ConfigureHost(WebApplication host)
	{
		var dbContext = host.Services.GetRequiredService<ApplicationDbContext>();
		dbContext.Database.Migrate();
		
		var logger = host.Services.GetRequiredService<ILogger<Program>>();
		var applicationStateTransmitter = host.Services.GetRequiredService<IApplicationStateTransmitter>();
		applicationStateTransmitter.NotifyConfigurationDone();

		host.UseRouting();
		host.UseGrpcWeb(new GrpcWebOptions() {DefaultEnabled = true});

		host.UseStaticFiles();

		host.UseAuthentication();
		host.UseAuthorization();

		host.MapHealthChecks("/health");

		host.MapGrpcService<PingService>();
		host.MapGrpcService<VoiceRecognitionService>();
		host.MapGrpcService<DesktopIntegrationServiceBridge>();

		
#if DEBUG
		host.MapGet("/download/test", (context) => context.RequestServices.GetRequiredService<IWwwFileLoader>().GetTestFile().ExecuteAsync(context));
#endif

		host.MapGet("/download/android", (context) => context.RequestServices.GetRequiredService<IWwwFileLoader>().GetAndroidApp().ExecuteAsync(context));
		host.MapGet("/qrcode/{url}", delegate(HttpContext context, string url)
		{
			return TypedResults.File(context.RequestServices.GetRequiredService<IQrCodeImageProvider>().GetQrCode(url), contentType: "image/png");
		});


		host.MapGet("/env", (IHostEnvironment env) => env.EnvironmentName);
		host.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

		host.MapRazorPages();
		
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

		builder.Services.AddDbContext<ApplicationDbContext>(options =>
			options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")!.Replace("{AppDir}",Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))));

		var tokenValidationParameters = new TokenValidationParameters();
		tokenValidationParameters.ValidateIssuer = true;
		tokenValidationParameters.ValidateAudience = true;
		tokenValidationParameters.ValidateLifetime = true;
		tokenValidationParameters.ValidateIssuerSigningKey = true;
		tokenValidationParameters.ValidIssuer = builder.Configuration["ApplicationSettings:Jwt:Issuer"];
		tokenValidationParameters.ValidAudience = builder.Configuration["ApplicationSettings:Jwt:Issuer"];
		tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["ApplicationSettings:Jwt:Key"]!));
		builder.Services.AddSingleton(tokenValidationParameters);
		
		// Add services to the container.
		builder.Services.AddHealthChecks()
			.AddCheck<AgentConnectivityCheck>(nameof(AgentConnectivityCheck));
		
		builder.Services.AddLogging();
		builder.Services.AddGrpc();
		builder.Services.AddGrpcReflection();
		builder.Services.AddRazorPages();

		builder.Services.AddAuthentication();
		builder.Services.AddAuthorization(options =>
		{
			options.AddPolicy(PolicyNames.ApiPolicy, policy =>
			{
				policy
					.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme, JwtBearerDefaults.AuthenticationScheme)
					.RequireAuthenticatedUser();
			});
			options.AddPolicy(PolicyNames.ApiPolicy, policy =>
			{
				policy
					.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme, JwtBearerDefaults.AuthenticationScheme)
					.RequireAuthenticatedUser();
			});
		});
		builder.Services.AddWindowsService();

		builder.Services.AddHostedService<DesktopIntegrationLauncherServiceDelegate>();
		builder.Services.AddHostedService<ClientDiscoveryDelegate>();
		
		builder.Services.AddSingleton<IWwwFileLoader, WwwFileLoader>();
		builder.Services.AddSingleton<IConnectedServerPorts, ConnectedServerPorts>();
		
		builder.Services				  
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
			.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
			{				   
				options.SaveToken = true;
				options.RefreshInterval = TimeSpan.TryParse(builder.Configuration["ApplicationSettings:Jwt:RefreshAccessTokenInterval"], out var parsedRefresh)
					? parsedRefresh 
					: TimeSpan.FromMinutes(1);
				options.TokenValidationParameters = tokenValidationParameters;

				options.Events = new JwtBearerEvents
				{
					OnAuthenticationFailed = context =>
					{
						if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
						{
							context.Response.Headers.Add("X-Token-Expired", "true");
						}

						return Task.CompletedTask;
					}
				};
			});

		builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
			{
				options.Password.RequireDigit = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.Password.RequiredLength = 3;
				options.Password.RequiredUniqueChars = 1;

				options.SignIn.RequireConfirmedAccount = true;
			})
			.AddDefaultTokenProviders()
			.AddUserManager<UserManager<ApplicationUser>>()
			.AddRoles<IdentityRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>();

		builder.Services.AddServiceApplicationModel();
		builder.Services.AddServiceIntegration();
	}

	private static void RegisterAsWindowsService(WebApplicationBuilder builder)
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			builder.Host.UseWindowsService();
	}
}