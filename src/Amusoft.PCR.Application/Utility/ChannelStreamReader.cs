using System.Threading.Channels;
using Grpc.Core;

namespace Amusoft.PCR.Application.Utility;

public class ChannelStreamReader<T> : IAsyncStreamReader<T>
	where T : notnull, new()
{
	private readonly ChannelReader<T> _reader;
	private T _current;

	public ChannelStreamReader(ChannelReader<T> reader)
	{
		_reader = reader;
		_current = new();
	}

	public async Task<bool> MoveNext(CancellationToken cancellationToken)
	{
		var success = await _reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false);
		if (success)
			_current = await _reader.ReadAsync(cancellationToken);
		return success;
	}

	public T Current => _current;
}