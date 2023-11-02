using System.Net;
using Amusoft.PCR.Application.Features.DesktopIntegration;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.Shared;
using Amusoft.PCR.Domain.Services;
using Amusoft.PCR.Domain.VM;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.Application.UI.VM;

public partial class HostViewModel : PageViewModel, INavigationCallbacks
{
	private readonly IDesktopIntegrationServiceFactory _integrationServiceFactory;

	public Task OnNavigatedToAsync()
	{
		return Task.CompletedTask;
	}
	
	private IPEndPoint? _address;
	private IDesktopIntegrationService? _client;

	protected override string GetDefaultPageTitle()
	{
		return "default";
	}

	public void Setup(HostItemViewModel viewModel)
	{
		Title = viewModel.Name;
		_address = viewModel.Connection;
		_client = _integrationServiceFactory.Create("http", viewModel.Connection);
	}

	[RelayCommand]
	public Task AbortShutdown()
	{
		return _client?.DesktopClient.AbortShutDownAsync() ?? Task.CompletedTask;
	}

	[RelayCommand]
	public Task Shutdown()
	{
		return _client?.DesktopClient.ShutDownDelayedAsync(true, TimeSpan.FromSeconds(60).Seconds) ?? Task.CompletedTask;
	}

	public HostViewModel(ITypedNavigator navigator, IDesktopIntegrationServiceFactory integrationServiceFactory) : base(navigator)
	{
		_integrationServiceFactory = integrationServiceFactory;
	}
}