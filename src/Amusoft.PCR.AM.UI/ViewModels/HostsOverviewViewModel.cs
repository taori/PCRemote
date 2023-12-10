#region

using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using Amusoft.PCR.Domain.Shared.Interfaces;
using Amusoft.Toolkit.Networking;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

#endregion

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class HostsOverviewViewModel : ReloadablePageViewModel, INavigationCallbacks
{
	private readonly ILogger<HostsOverviewViewModel> _logger;
	private readonly IBearerTokenStorage _bearerTokenStorage;
	private readonly IHostRepository _hostRepository;
	private readonly IToast _toast;
	private readonly ITypedNavigator _navigator;
	private readonly IDiscoveryMessageInterface _discoveryMessageInterface;

	[ObservableProperty]
	private bool _arePortsMissing;

	[ObservableProperty]
	private ObservableCollection<HostItemViewModel> _items = new();

	public HostsOverviewViewModel(ILogger<HostsOverviewViewModel> logger, IHostRepository hostRepository, IToast toast, ITypedNavigator navigator, IDiscoveryMessageInterface discoveryMessageInterface) : base(navigator)
	{
		_logger = logger;
		_hostRepository = hostRepository;
		_toast = toast;
		_navigator = navigator;
		_discoveryMessageInterface = discoveryMessageInterface;
	}

	protected override async Task OnReloadAsync(CancellationToken cancellationToken)
	{
		_logger.LogDebug("Loading hosts");
		Items = new ObservableCollection<HostItemViewModel>(await LoadHostsFromPortsAsync(cancellationToken));
	}

	[RelayCommand]
	public Task OpenHostAsync(HostItemViewModel viewModel)
	{
		return _navigator.OpenHost(viewModel.Connection, viewModel.Name, viewModel.Protocol);
	}

	[RelayCommand]
	public async Task ConfigureHostsAsync()
	{
		await Navigator.OpenSettings();
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.Page_Title_HostsOverview;
	}

	public Task OnNavigatedToAsync()
	{
		return ReloadAsync();
	}

	private HostItemViewModel[] GetHostItemModel(UdpReceiveResult result)
	{
		if (_discoveryMessageInterface.TryParse(result.Buffer, out var host) && host is {} h)
		{
			return h.Ports.Select(port => new HostItemViewModel(
				new(result.RemoteEndPoint.Address, port),
				$"{h.MachineName}",
				"http",
				item => _ = OpenHostAsync(item))
			).ToArray();
		}
		
		_logger.LogDebug("Received buffer contains no data about hosts");
		return Array.Empty<HostItemViewModel>();
	}

	private async Task<ICollection<HostItemViewModel>> LoadHostsFromPortsAsync(CancellationToken cancellationToken)
	{
		var ports = await _hostRepository.GetHostPortsAsync();
		var items = new List<HostItemViewModel>();
		await foreach (var udpReceiveResult in GetUdpReceiveResultsAsync(ports).WithCancellation(cancellationToken))
		{
			if(cancellationToken.IsCancellationRequested)
				continue;

			foreach (var hostItemViewModel in GetHostItemModel(udpReceiveResult))
			{
				if (cancellationToken.IsCancellationRequested)
					continue;

				_logger.LogDebug("Found host {Address}", hostItemViewModel.Connection);
				items.Add(hostItemViewModel);
			}
		}

		ArePortsMissing = ports.Length == 0;

		if(cancellationToken.IsCancellationRequested)
			_logger.LogDebug("Requeste cancelled - returning empty array.");

		return cancellationToken.IsCancellationRequested
			? Array.Empty<HostItemViewModel>()
			: items;
	}

	private IAsyncEnumerable<UdpReceiveResult> GetUdpReceiveResultsAsync(int[] ports)
	{
		return AsyncEnumerableEx.Merge(GetPortSourcesAsync(ports));

		async IAsyncEnumerable<IAsyncEnumerable<UdpReceiveResult>> GetPortSourcesAsync(int[] ports)
		{
			var portRange = Math.Pow(2, 16);
			foreach (var port in ports.Where(d => d < portRange))
			{
				var duration = TimeSpan.FromSeconds(1);
				using var cts = new CancellationTokenSource(duration);
				using var session = new UdpBroadcastSession(new UdpBroadcastCommunicationChannelSettings(port), cts.Token);
				await session.BroadcastAsync(Encoding.UTF8.GetBytes(_discoveryMessageInterface.DiscoveryRequestMessage), cts.Token);
				yield return session.GetResponsesAsync(duration);
			}
		}
	}
}

public partial class HostItemViewModel : ObservableObject
{
	public readonly IPEndPoint Connection;
	public readonly string Protocol;

	[ObservableProperty]
	private string _name;

	[ObservableProperty]
	private Action<HostItemViewModel>? _callback;

	internal HostItemViewModel(IPEndPoint connection, string name, string protocol, Action<HostItemViewModel>? callback)
	{
		Protocol = protocol;
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