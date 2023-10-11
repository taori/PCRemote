using CommunityToolkit.Mvvm.ComponentModel;

namespace Amusoft.PCR.Application.Shared;

public abstract partial class PageViewModel : ObservableObject
{
	[ObservableProperty]
	private string? _title;

	protected PageViewModel()
	{
		Title = GetDefaultPageTitleImpl();
	}

	private string GetDefaultPageTitleImpl() => GetDefaultPageTitle();

	protected abstract string GetDefaultPageTitle();
}