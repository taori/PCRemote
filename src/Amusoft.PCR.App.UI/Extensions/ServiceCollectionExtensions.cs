using Amusoft.PCR.App.UI.Pages;

namespace Amusoft.PCR.App.UI.Extensions;

public static class ServiceCollectionExtensions
{
	public static void AddUIViews(this IServiceCollection services)
	{
		services.AddTransient<CommandButtonList>();
		services.AddTransient<MouseControl>();
		services.AddTransient<LogsSettings>();
		services.AddTransient<HostAccounts>();
		services.AddTransient<HostAccountPermissions>();

		services.AddTransient<Debug>();
		services.AddTransient<MainPage>();
		services.AddTransient<Logs>();
		services.AddTransient<Audio>();
		services.AddTransient<Programs>();
		services.AddTransient<InputControl>();
		services.AddTransient<Monitors>();
		services.AddTransient<SystemState>();
		services.AddTransient<HostsOverview>();
		services.AddTransient<Settings>();
		services.AddTransient<Host>();
	}
}