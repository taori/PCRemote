using System.Diagnostics;
using NLog;

namespace Amusoft.PCR.App.WindowsAgent.Interop;


internal static class ProcessExitListenerManager
{
	private static readonly Logger Log = LogManager.GetCurrentClassLogger();

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

			process.EnableRaisingEvents = true;
			process.Exited += processOnExited;

			_ = Task.Run(async () =>
			{
				// self termination if parent process has been killed by force
				while (true)
				{
					if (IsProcessAlive(processId))
					{
						ProcessExited?.Invoke(null, processId);
					}

					await Task.Delay(15000);
				}
			});
			return true;
		}
		catch (Exception e)
		{
			Log.Error(e, "Failed to observe process {Id}", processId);
			return false;
		}
	}

	private static bool IsProcessAlive(int processId)
	{
		try
		{

			return !Process.GetProcessById(processId).HasExited;
		}
		catch (Exception)
		{
			return false;
		}
	}
}