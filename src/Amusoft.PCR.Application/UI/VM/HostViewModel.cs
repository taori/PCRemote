using System.Net;
using Amusoft.PCR.Application.Shared;
using Amusoft.PCR.Domain.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Amusoft.PCR.Application.UI.VM;

public partial class HostViewModel : PageViewModel
{
	private readonly INavigation _navigation;

	public HostViewModel(INavigation navigation)
	{
		_navigation = navigation;
	}
	
	private IPEndPoint? _address;

	protected override string GetDefaultPageTitle()
	{
		return "just a test";
	}

	public void Setup(HostItemViewModel viewModel)
	{
		Title = viewModel.Name;
		_address = viewModel.Connection;
	}
}