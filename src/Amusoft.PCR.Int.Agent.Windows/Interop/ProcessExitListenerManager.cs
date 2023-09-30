using System;
using System.Diagnostics;
using NLog;

namespace Amusoft.PCR.Int.Agent.Windows.Interop;


internal static class ProcessExitListenerManager
{
	private static readonly Logger Log = LogManager.GetLogger(nameof(ProcessExitListenerManager));

	public static event EventHandler<int>? ProcessExited;

	public static bool TryObserveProcessExit(int processId)
	{
		try
		{
			var process = Process.GetProcessById(processId);
			Log.Debug("Observing process {Id} for exit", processId);
			EventHandler? processOnExited = default;
			processOnExited = (sender, args) =>
			{
				ProcessExited?.Invoke(null, processId);
				process.Exited -= processOnExited;
			};
			process.Exited += processOnExited;
			return true;
		}
		catch (Exception e)
		{
			Log.Error(e, "Failed to observe process {Id}", processId);
			return false;
		}
	}
}