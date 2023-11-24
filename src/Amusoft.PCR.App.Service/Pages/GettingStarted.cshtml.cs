using System.Net.NetworkInformation;
using System.Net.Sockets;
using Amusoft.PCR.AM.Service.Features;
using Amusoft.PCR.AM.Service.Features.GettingStarted;
using Amusoft.PCR.Domain.AgentSettings;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

namespace Amusoft.PCR.App.Service.Pages;

public class GettingStarted : PageModel
{
	public GettingStarted(ServerEndpointProvider serverEndpointProvider)
	{
		ServerEndpointProvider = serverEndpointProvider;
	}
	
	public ServerEndpointProvider ServerEndpointProvider { get; set; }
	
	public List<ServerEndpointProvider.StatusModel> Endpoints { get; set; }
	
	public async Task OnGetAsync()
	{
		Endpoints = await ServerEndpointProvider.GetStatusModelsAsync();
	}
}