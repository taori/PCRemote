using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Channels;
using NLog;

namespace Amusoft.Toolkit.Networking;

public class UdpBroadcastSession : IDisposable, IUdpBroadcastChannel
{
	private static readonly NLog.ILogger Log = NLog.LogManager.GetCurrentClassLogger();
	
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
		Log.Trace("Received from {Address}", obj.RemoteEndPoint);
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

		using var sub = _buffer
			.DistinctUntilChanged(d => d.RemoteEndPoint)
			.TakeUntil(DateTimeOffset.Now.Add(duration))
			.Subscribe(
				result =>
				{
					Log.Trace("Pipe written from {Address}", result.RemoteEndPoint);
					pipe.Writer.TryWrite(result);
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