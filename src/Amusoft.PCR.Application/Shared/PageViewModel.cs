using Amusoft.PCR.Application.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.Application.Shared;

public abstract partial class PageViewModel : ObservableObject
{
	protected readonly ITypedNavigator Navigator;

	[ObservableProperty]
	private string? _title;

	protected PageViewModel(ITypedNavigator navigator)
	{
		Navigator = navigator;
		Title = GetDefaultPageTitleImpl();
	}

	private string GetDefaultPageTitleImpl() => GetDefaultPageTitle();

	protected abstract string GetDefaultPageTitle();

	[RelayCommand(AllowConcurrentExecutions = false)]
	public Task GoBack()
	{
		return Navigator.PopAsync();
	}
}