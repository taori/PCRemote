﻿using Amusoft.PCR.AM.Agent.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.AM.Agent;


public static class ServiceCollectionExtensions
{
	public static void AddWindowsAgentApplicationModel(this IServiceCollection services)
	{
		services.AddTransient<ConfirmWindowViewModel>();
		services.AddTransient<PromptWindowModel>();
	}
	
}