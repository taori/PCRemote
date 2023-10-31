using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Channels;

namespace Amusoft.Toolkit.Networking;

public class UdpBroadcastSession : IDisposable, IUdpBroadcastChannel
{
	private readonly UdpBroadcastCommunicationChannel _channel;

	private readonly ReplaySubject<UdpReceiveResult> _buffer = new();

	public UdpBroadcastSession(UdpBroadcastCommunicationChannelSettings settings, CancellationToken cancellationToken)
	{
		_channel = new UdpBroadcastCommunicationChannel(settings);
		_channel.MessageReceived
			.Subscribe(OnNext);
		_channel.StartListening(cancellationToken);
	}

	private void OnNext(UdpReceiveResult obj)
	{
		_buffer.OnNext(obj);
	}

	public async Task<List<UdpReceiveResult>> GetAllResponsesAsync(TimeSpan duration, CancellationToken cancellationToken)
	{
		var results = new List<UdpReceiveResult>();
		try
		{
			await foreach (var item in GetResponsesAsync(duration).WithCancellation(cancellationToken))
			{
				results.Add(item);
			}
		}
		catch (OperationCanceledException) { }

		return results;
	}

	public async IAsyncEnumerable<UdpReceiveResult> GetResponsesAsync(TimeSpan duration)
	{
		var pipe = Channel.CreateUnbounded<UdpReceiveResult>();

		using var sub = Observable
			.Interval(TimeSpan.FromMilliseconds(50))
			.CombineLatest(_buffer)
			.DistinctUntilChanged(d => d.Second)
			.TakeUntil(DateTimeOffset.Now.Add(duration))
			.Subscribe(
				result =>
				{
					pipe.Writer.TryWrite(result.Second);
				},
				() =>
				{
					pipe.Writer.TryComplete();
				}
			);

		await foreach (var item in pipe.Reader.ReadAllAsync())
		{
			yield return item;
		}
	}

	public void Dispose()
	{
		_channel.Dispose();
	}

	public Task<bool> BroadcastAsync(byte[] bytes, CancellationToken cancellationToken)
	{
		return _channel.BroadcastAsync(bytes, cancellationToken);
	}

	public Task<bool> SendToAsync(byte[] bytes, IPEndPoint endPoint, CancellationToken cancellationToken)
	{
		return _channel.SendToAsync(bytes, endPoint, cancellationToken);
	}
}