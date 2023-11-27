using System.Collections.ObjectModel;
using System.Windows.Input;
using Amusoft.PCR.AM.UI.Extensions;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using Amusoft.PCR.Int.IPC;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Translations = Amusoft.PCR.AM.Shared.Resources.Translations;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class AudioViewModel : ReloadablePageViewModel, INavigationCallbacks
{
	private readonly HostViewModel _hostViewModel;

	[ObservableProperty]
	private ObservableCollection<AudioViewModelItem>? _items;

	protected override async Task OnReloadAsync(CancellationToken cancellationToken)
	{
		var response = await _hostViewModel.DesktopIntegrationClient.Desktop(f => f.GetAudioFeedsResponse()) ?? new AudioFeedResponse();
		var items = new ObservableCollection<AudioViewModelItem>();
		if (response is {Items.Count: > 0})
		{
			foreach (var item in response.Items)
			{
				items.Add(new AudioViewModelItem()
				{
					Id = item.Id,
					Muted = item.Muted,
					Name = item.Name,
					Volume = item.Volume,
					MuteCommand = new AsyncRelayCommand(() => ToggleItemMute(item.Id)),
					VolumeChangedCommand = new AsyncRelayCommand(() => UpdateVolume(item.Id)),
				});
			}
		}

		Items = items;
	}

	private async Task UpdateVolume(string itemId)
	{
		if (await _hostViewModel.DesktopIntegrationClient.Desktop(f => f.GetAudioFeedsResponse()) is { Success: true } feed)
		{
			if (feed.Items.FirstOrDefault(d => d.Id == itemId) is { } feedItem 
			    && Items?.FirstOrDefault(f => f.Id == itemId) is {} sliderItem)
			{
				feedItem.Volume = sliderItem.Volume;

				await _hostViewModel.DesktopIntegrationClient.Desktop(f => f.UpdateAudioFeed(new UpdateAudioFeedRequest()
				{
					Item = feedItem,
				}));
			}
		}
	}

	private async Task ToggleItemMute(string itemId)
	{
		if (await _hostViewModel.DesktopIntegrationClient.Desktop(f => f.GetAudioFeedsResponse()) is {Success: true} feed)
		{
			if (feed.Items.FirstOrDefault(d => d.Id == itemId) is {} dataItem
			    && Items?.FirstOrDefault(d => d.Id == itemId) is {} viewItem)
			{
				var updateItem = dataItem.Clone();
				updateItem.Muted = !dataItem.Muted;
				
				var update = await _hostViewModel.DesktopIntegrationClient.Desktop(f => f.UpdateAudioFeed(new UpdateAudioFeedRequest()
				{
					Item = updateItem,
				}));

				if (update is {Success: true})
				{
					viewItem.Muted = updateItem.Muted;
				}
			}
		}
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.Page_Title_Audio;
	}

	public AudioViewModel(ITypedNavigator navigator, HostViewModel hostViewModel) : base(navigator)
	{
		_hostViewModel = hostViewModel;
	}

	public Task OnNavigatedToAsync()
	{
		return ReloadAsync();
	}
}

public partial class AudioViewModelItem : ObservableObject
{
	public string Icon => Muted ? "volume_off.png" : "volume_up.png";

	[ObservableProperty]
	private string? _name;

	[ObservableProperty]
	private string? _id;

	[ObservableProperty]
	private float _volume;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(Icon))]
	private bool _muted;

	[ObservableProperty]
	private ICommand? _muteCommand;

	[ObservableProperty]
	private ICommand? _volumeChangedCommand;
}