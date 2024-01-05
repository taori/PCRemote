using System.Threading.Channels;
using Amusoft.PCR.AM.UI.Interfaces;
using Grpc.Core;
using NLog;
using NLog.Targets;
using Channel = System.Threading.Channels.Channel;

namespace Amusoft.PCR.Int.UI;

public class ToastTarget : Target
{
	private readonly IToastable _toast;

	private readonly Channel<LogEventInfo> _events;

	private readonly CancellationTokenSource _cts;

	public ToastTarget()
	{
		_toast = new Toast().Make(string.Empty, false);
		_events = Channel.CreateUnbounded<LogEventInfo>();
		_cts = new CancellationTokenSource();
		Task.Run(StartPipeWriter);
	}

	protected override void Dispose(bool disposing)
	{
		_cts.Dispose();
		base.Dispose(disposing);
	}

	private async Task StartPipeWriter()
	{
		await foreach (var eventItem in _events.Reader.ReadAllAsync(_cts.Token))
		{
			await _toast.SetText(eventItem.Message).Show();
			await Task.Delay(5000);
		}
	}

	protected override void Write(LogEventInfo logEvent)
	{
		if (logEvent.Exception is RpcException rex)
		{
			_events.Writer.TryWrite(new LogEventInfo(logEvent.Level, logEvent.LoggerName, $"Grpc Status: ({rex.StatusCode.ToString()})"));
		}
		else
		{
			_events.Writer.TryWrite(logEvent);
		}
	}
}