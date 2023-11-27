using System.Net;
using Amusoft.PCR.Application.Features.DesktopIntegration;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.Shared;
using Amusoft.PCR.Domain.VM;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.Application.UI.VM;

public partial class HostViewModel : PageViewModel, INavigationCallbacks
{
	private readonly ITypedNavigator _navigator;
	private readonly IDesktopIntegrationServiceFactory _integrationServiceFactory;

	public Task OnNavigatedToAsync()
	{
		return Task.CompletedTask;
	}
	
	private IPEndPoint? _address;
	public IDesktopIntegrationService? DesktopIntegrationClient { get; set; }

	protected override string GetDefaultPageTitle()
	{
		return "default";
	}

	public void Setup(HostItemViewModel viewModel)
	{
		Title = viewModel.Name;
		_address = viewModel.Connection;
		DesktopIntegrationClient = _integrationServiceFactory.Create("http", viewModel.Connection);
	}

	[RelayCommand]
	public Task OpenAudio()
	{
		return _navigator.ScopedNavigationAsync(d => d.AddSingleton(this), d => d.OpenAudio());
	}

	[RelayCommand]
	public Task OpenSystemState()
	{
		return _navigator.ScopedNavigationAsync(d => d.AddSingleton(this), d => d.OpenSystemState());
	}

	[RelayCommand]
	public Task OpenMonitors()
	{
		return _navigator.ScopedNavigationAsync(d => d.AddSingleton(this), d => d.OpenMonitors());
	}

	[RelayCommand]
	public Task OpenInputControl()
	{
		return _navigator.ScopedNavigationAsync(d => d.AddSingleton(this), d => d.OpenInputControl());
	}

	[RelayCommand]
	public Task OpenPrograms()
	{
		return _navigator.ScopedNavigationAsync(d => d.AddSingleton(this), d => d.OpenPrograms());
	}

	public HostViewModel(ITypedNavigator navigator, IDesktopIntegrationServiceFactory integrationServiceFactory) : base(navigator)
	{
		_navigator = navigator;
		_integrationServiceFactory = integrationServiceFactory;
	}
}