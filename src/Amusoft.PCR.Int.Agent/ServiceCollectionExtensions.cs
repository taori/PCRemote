using Amusoft.PCR.AM.Agent.Interfaces;
using Amusoft.PCR.Int.Agent.Dependencies;
using Amusoft.PCR.Int.IPC;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.Int.Agent;

public static class ServiceCollectionExtensions
{
	public static void AddAgentIntegration(this IServiceCollection services)
	{
		services.AddLogging();
		
		services.AddSingleton<IInterprocessCommunicationServer, InterprocessCommunicationServer>();
		services.AddSingleton<IApplicationHost, ApplicationHost>();
		
		services.AddSingleton<DesktopIpcAgentLayer>();
		services.AddSingleton<VoiceRecognitionIpcAgentLayer>();
	}
	
}