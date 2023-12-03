using Amusoft.PCR.AM.Service.Features.GettingStarted;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Amusoft.PCR.App.Service.Pages;

public class GettingStarted : PageModel
{
	public GettingStarted(ServerEndpointProvider serverEndpointProvider)
	{
		ServerEndpointProvider = serverEndpointProvider;
	}
	
	public ServerEndpointProvider ServerEndpointProvider { get; set; }
	
	public List<ServerEndpointProvider.StatusModel>? Endpoints { get; set; }
	
	public async Task OnGetAsync()
	{
		Endpoints = await ServerEndpointProvider.GetStatusModelsAsync();
	}
}