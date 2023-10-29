using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using Amusoft.PCR.Application.Features.DesktopIntegration;
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
				if (GrpcHandshakeFormatter.Parse(result.Buffer) is { } host)
				{
					foreach (var hostPort in host.Ports)
					{
						Items.Add(new HostItemViewModel(new(result.RemoteEndPoint.Address, hostPort), $"{host.MachineName}:{hostPort}", () => _toast.Make("it works").Show()));
					}
				}
					
			});
		_channel.StartListening(CancellationToken.None);
	}

	protected override async Task OnReloadAsync()
	{
		Items.Clear();
		await _channel.SendAsync(Encoding.UTF8.GetBytes(GrpcHandshakeClientMessage.Message));
	}

	[RelayCommand]
	public async Task ConfigureHostsAsync()
	{
		var ports = await _hostRepository.GetHostPortsAsync();
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
	public readonly IPEndPoint Connection;

	[ObservableProperty]
	private string _name;

	[ObservableProperty]
	private Action? _callback;

	internal HostItemViewModel(IPEndPoint connection, string name, Action? callback)
	{
		Connection = connection;
		Name = name;
		Callback = callback;
	}

	[RelayCommand]
	protected void OnExecuteCallback()
	{
		Callback?.Invoke();
	}
}