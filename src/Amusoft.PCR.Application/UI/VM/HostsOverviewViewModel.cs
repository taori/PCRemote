using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using Amusoft.PCR.Application.Features.DesktopIntegration;
using Amusoft.PCR.Application.Resources;
using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.UI.Repos;
using Amusoft.PCR.Domain.Services;
using Amusoft.PCR.Domain.VM;
using Amusoft.Toolkit.Networking;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Application.UI.VM;

public partial class HostsOverviewViewModel : Shared.ReloadablePageViewModel, INavigationCallbacks
{
	private readonly ILogger<HostsOverviewViewModel> _logger;
	private readonly HostRepository _hostRepository;
	private readonly IToast _toast;
	private readonly ITypedNavigator _navigator;

	[ObservableProperty]
	private bool _arePortsMissing;

	[ObservableProperty]
	private ObservableCollection<HostItemViewModel> _items = new();

	public HostsOverviewViewModel(ILogger<HostsOverviewViewModel> logger, HostRepository hostRepository, IToast toast, ITypedNavigator navigator) : base(navigator)
	{
		_logger = logger;
		_hostRepository = hostRepository;
		_toast = toast;
		_navigator = navigator;
	}

	protected override async Task OnReloadAsync(CancellationToken cancellationToken)
	{
		_logger.LogDebug("Loading hosts");
		
		Items = new ObservableCollection<HostItemViewModel>(await LoadHostsFromPortsAsync(cancellationToken));
	}

	[RelayCommand]
	public async Task OpenHostAsync(HostItemViewModel viewModel)
	{
		await _navigator.OpenHost(d => d.Setup(viewModel));
		// await _toast.Make($"Connecting to {viewModel.Connection.ToString()}").Show();
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
		if (GrpcHandshakeFormatter.Parse(result.Buffer) is { } host)
		{
			return host.Ports.Select(port => new HostItemViewModel(
				new(result.RemoteEndPoint.Address, port),
				$"{host.MachineName}",
				item => _ = OpenHostAsync(item))
			).ToArray();
		}
		
		return Array.Empty<HostItemViewModel>();
	}

	private async Task<ICollection<HostItemViewModel>> LoadHostsFromPortsAsync(CancellationToken cancellationToken)
	{
		var ports = await _hostRepository.GetHostPortsAsync();
		var items = new List<HostItemViewModel>();
		await foreach (var udpReceiveResult in GetUdpReceiveResults(ports).WithCancellation(cancellationToken))
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

	private static IAsyncEnumerable<UdpReceiveResult> GetUdpReceiveResults(int[] ports)
	{
		return AsyncEnumerableEx.Merge(GetPortSourcesAsync(ports));

		static async IAsyncEnumerable<IAsyncEnumerable<UdpReceiveResult>> GetPortSourcesAsync(int[] ports)
		{
			var limit = Math.Pow(2, 16);
			foreach (var port in ports.Where(d => d < limit))
			{
				var duration = TimeSpan.FromSeconds(1);
				using var cts = new CancellationTokenSource(duration);
				using var session = new UdpBroadcastSession(new UdpBroadcastCommunicationChannelSettings(port), cts.Token);
				await session.BroadcastAsync(Encoding.UTF8.GetBytes(GrpcHandshakeClientMessage.Message), cts.Token);
				yield return session.GetResponsesAsync(duration);
			}
		}
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