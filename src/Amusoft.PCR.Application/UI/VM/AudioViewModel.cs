using System.Collections.ObjectModel;
using Amusoft.PCR.Application.Resources;
using Amusoft.PCR.Application.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Amusoft.PCR.Application.UI.VM;

public partial class AudioViewModel : Shared.ReloadablePageViewModel
{
	[ObservableProperty]
	private ObservableCollection<AudioViewModelItem>? _items;

	protected override async Task OnReloadAsync()
	{
		await Task.Delay(5000);
		Items = new ObservableCollection<AudioViewModelItem>();
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.Page_Title_Audio;
	}

	public AudioViewModel(ITypedNavigator navigator) : base(navigator)
	{
	}
}

public partial class AudioViewModelItem : ObservableObject
{

}