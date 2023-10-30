using System.Collections.ObjectModel;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using Amusoft.PCR.Application.Features.DesktopIntegration;
using Amusoft.PCR.Application.Resources;
using Amusoft.PCR.Application.UI.Repos;
using Amusoft.PCR.Domain.Services;
using Amusoft.PCR.Domain.VM;
using Amusoft.Toolkit.Networking;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.Application.UI.VM;

public partial class HostsOverviewViewModel : Shared.ReloadablePageViewModel, INavigationCallbacks
{
	private readonly HostRepository _hostRepository;
	private readonly IToast _toast;
	private readonly INavigation _navigation;
	private readonly IServiceProvider _serviceProvider;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(IsErrorLabelVisible))]
	private ObservableCollection<HostItemViewModel> _items = new();

	private UdpBroadcastCommunicationChannel _channel;

	public bool IsErrorLabelVisible
	{
		get => Items?.Count == 0;
	}

	public HostsOverviewViewModel(HostRepository hostRepository, IToast toast, INavigation navigation, IServiceProvider serviceProvider)
	{
		_hostRepository = hostRepository;
		_toast = toast;
		_navigation = navigation;
		_serviceProvider = serviceProvider;
		_channel = new UdpBroadcastCommunicationChannel(new UdpBroadcastCommunicationChannelSettings(50001));
		_channel.MessageReceived
			.Subscribe(result =>
			{
				if (GrpcHandshakeFormatter.Parse(result.Buffer) is { } host)
				{
					foreach (var hostPort in host.Ports)
					{
						Items.Add(new HostItemViewModel(
							new(result.RemoteEndPoint.Address, hostPort), 
							$"{host.MachineName}:{hostPort}", 
							item => OpenHostAsync(item))
						);
					}
				}
					
			});
		_channel.StartListening(CancellationToken.None);
	}

	protected override async Task OnReloadAsync()
	{
		Items.Clear();
		await _channel.BroadcastAsync(Encoding.UTF8.GetBytes(GrpcHandshakeClientMessage.Message));
	}

	[RelayCommand]
	public Task OpenHostAsync(HostItemViewModel viewModel)
	{
		var hostViewModel = _serviceProvider.GetRequiredService<HostViewModel>();
		hostViewModel.Setup(viewModel);
		// _navigation.GoToAsync("/host", hostViewModel);
		return _toast.Make($"Connecting to {viewModel.Connection.ToString()}").Show();
	}

	[RelayCommand]
	public async Task ConfigureHostsAsync()
	{
		var ports = await _hostRepository.GetHostPortsAsync();
		await _channel.BroadcastAsync(Encoding.UTF8.GetBytes(GrpcHandshakeClientMessage.Message));
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
	private Action<HostItemViewModel>? _callback;

	internal HostItemViewModel(IPEndPoint connection, string name, Action<HostItemViewModel>? callback)
	{
		Connection = connection;
		Name = name;
		Callback = callback;
	}

	[RelayCommand]
	protected void OnExecuteCallback(HostItemViewModel item)
	{
		Callback?.Invoke(item);
	}
}