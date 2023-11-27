using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Channels;
using Shouldly;

namespace Amusoft.Toolkit.UnitTests;

public class UdpBroadcastSessionTests
{
	[Fact]
	public async Task VerifyAlgorithm()
	{
		var buffer = new ReplaySubject<int>(); 
		buffer.OnNext(1);
		buffer.OnNext(2);
		buffer.OnNext(3);
		
		var pipe = Channel.CreateUnbounded<int>();
		using var sub = buffer
			.TakeUntil(DateTimeOffset.Now.Add(TimeSpan.FromSeconds(3)))
			.DistinctUntilChanged()
			.Subscribe(
				result =>
				{
					pipe.Writer.TryWrite(result);
				},
				() =>
				{
					pipe.Writer.TryComplete();
				}
			);
		
		buffer.OnNext(4);
		buffer.OnNext(5);

		var results = new List<int>();
		await foreach (var item in pipe.Reader.ReadAllAsync())
		{
			results.Add(item);
		}
		
		results.Count.ShouldBe(5);
	}
}