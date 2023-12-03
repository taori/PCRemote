using Amusoft.PCR.AM.UI.ViewModels;
using Amusoft.PCR.App.UI.Pages;

namespace Amusoft.PCR.App.UI.Extensions;

public static class ServiceCollectionExtensions
{
	public static void AddUIViews(this IServiceCollection services)
	{
		services.AddTransient<CommandButtonList>();
		services.AddTransient<MouseControl>();
		
		services.AddScoped<Debug>();
		services.AddScoped<MainPage>();
		services.AddScoped<Logs>();
		services.AddScoped<Audio>();
		services.AddScoped<Programs>();
		services.AddScoped<InputControl>();
		services.AddScoped<Monitors>();
		services.AddScoped<SystemState>();
		services.AddScoped<HostsOverview>();
		services.AddScoped<Settings>();
		services.AddScoped<Host>();
	}
}