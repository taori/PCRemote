using System.Net;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.Shared;
using Amusoft.PCR.Domain.Services;
using Amusoft.PCR.Domain.VM;

namespace Amusoft.PCR.Application.UI.VM;

public partial class HostViewModel : PageViewModel, INavigationCallbacks
{
	public Task OnNavigatedToAsync()
	{
		return Task.CompletedTask;
	}
	
	private IPEndPoint? _address;

	protected override string GetDefaultPageTitle()
	{
		return "default";
	}

	public void Setup(HostItemViewModel viewModel)
	{
		Title = viewModel.Name;
		_address = viewModel.Connection;
	}

	public HostViewModel(ITypedNavigator navigator) : base(navigator)
	{
	}
}