using Amusoft.PCR.AM.Agent.Interfaces;
using Amusoft.PCR.Int.Agent;
using Amusoft.PCR.Int.WindowsAgent.Dependencies;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.Int.WindowsAgent;

public static class ServiceCollectionExtensions
{
	public static void AddWindowsAgentIntegration(this IServiceCollection services)
	{
		services.AddAgentIntegration();

		services.AddSingleton<IApplicationController, ApplicationController>();
		services.AddSingleton<IDesktopIntegrationProcessor, DesktopIntegrationProcessor>();
		services.AddSingleton<IVoiceRecognitionProcessor, VoiceRecognitionProcessor>();
	}
}