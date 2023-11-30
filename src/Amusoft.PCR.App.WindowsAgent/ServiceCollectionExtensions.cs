using Amusoft.PCR.AM.Agent.Interfaces;
using Amusoft.PCR.App.WindowsAgent.Dependencies;
using Amusoft.PCR.App.WindowsAgent.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.App.WindowsAgent;

public static class ServiceCollectionExtensions
{
	public static void AddWindowsAgentCore(this IServiceCollection services)
	{
		services.AddSingleton<IAgentUserInterface, AgentUserInterface>();
		services.AddScoped<PromptWindow>();
		services.AddScoped<ConfirmWindow>();
	}
}