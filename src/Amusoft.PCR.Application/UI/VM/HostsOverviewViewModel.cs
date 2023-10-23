using System.Collections.ObjectModel;
using System.Text;
using Amusoft.PCR.Application.Resources;
using Amusoft.PCR.Application.UI.Repos;
using Amusoft.PCR.Domain.Services;
using Amusoft.PCR.Domain.VM;
using Amusoft.Toolkit.Networking;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.Application.UI.VM;

public partial class HostsOverviewViewModel : Shared.ReloadablePageViewModel, INavigationCallbacks
{
	private readonly HostRepository _hostRepository;
	private readonly IToast _toast;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsErrorLabelVisible))]
	private ObservableCollection<HostItemViewModel> _items = new();

	private UdpBroadcastCommunicationChannel _channel;

	public bool IsErrorLabelVisible
	{
		get => Items?.Count == 0;
	}

	public HostsOverviewViewModel(HostRepository hostRepository, IToast toast)
	{
		_hostRepository = hostRepository;
		_toast = toast;
		_channel = new UdpBroadcastCommunicationChannel(new UdpBroadcastCommunicationChannelSettings(50001));
		_channel.MessageReceived
			.Subscribe(result =>
			{
				Items.Add(new HostItemViewModel(Encoding.UTF8.GetString(result.Buffer), () => _toast.Make("it works").Show()));
			});
		_channel.StartListening(CancellationToken.None);
	}

	protected override async Task OnReloadAsync()
	{
		var ports = await _hostRepository.GetHostPortsAsync();
		Items = new ObservableCollection<HostItemViewModel>(ports.Select(d => new HostItemViewModel(d.ToString(), () => {})));
	}

	[RelayCommand]
	public async Task ConfigureHostsAsync()
	{
		await _channel.SendAsync(Encoding.UTF8.GetBytes("Test of UDP broadcast"));
		// Items.Add(new HostItemViewModel("Test", () => _toast.Make("hello").Show()));
		// return Task.CompletedTask;
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.Page_Title_HostsOverview;
	}

	public Task OnNavigatedToAsync()
	{
		return OnReloadAsync();
	}
}

public partial class HostItemViewModel : ObservableObject
{
	[ObservableProperty]
	private string _name;

	[ObservableProperty]
	private Action? _callback;

	public HostItemViewModel(string name, Action? callback)
	{
		Name = name;
		Callback = callback;
	}

	[RelayCommand]
	protected void OnExecuteCallback()
	{
		Callback?.Invoke();
	}
}