using System.Collections.ObjectModel;
using Amusoft.PCR.Application.Resources;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Amusoft.PCR.Application.UI;

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
}

public partial class AudioViewModelItem : ObservableObject
{

}