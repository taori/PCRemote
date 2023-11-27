using System.Diagnostics;
using System.IO;
using Amusoft.PCR.App.WindowsAgent.Interop;
using Amusoft.PCR.Int.IPC;
using NLog;

namespace Amusoft.PCR.App.WindowsAgent.Helpers;

public class ProcessHelper
{
	private static readonly Logger Log = LogManager.GetCurrentClassLogger();

	public static bool TryKillProcess(int processId)
	{
		// Logger.Info($"Killing process [{concrete.ProcessId}].");
		var process = Process.GetProcesses().FirstOrDefault(d => d.Id == processId);
		if (process == null)
		{
			Log.Warn($"Process id [{processId}] not found.");
			return false;
		}

		try
		{
			process.Kill();
			return true;
		}
		catch (Exception e)
		{
			Log.Error(e);
			return false;
		}
	}

	public static bool TryGetProcessList(out List<ProcessListResponseItem> items)
	{
		items = new List<ProcessListResponseItem>();
		try
		{
			var processes = Process.GetProcesses();
			foreach (var process in processes)
			{
				items.Add(new ProcessListResponseItem()
				{
					ProcessId = process.Id,
					ProcessName = process.ProcessName,
					MainWindowTitle = process.MainWindowTitle
				});
			}
			return true;
		}
		catch (Exception e)
		{
			Debug.WriteLine(e);
			return false;
		}
	}

	public static bool TryLaunchProgram(string program, string? arguments = default)
	{
		try
		{
			var process = new Process();
			process.StartInfo = string.IsNullOrEmpty(arguments)
				? new ProcessStartInfo(program)
				: new ProcessStartInfo(program, arguments);

			process.Start();
			return true;
		}
		catch (Exception e)
		{
			Log.Error(e, "Exception occured while calling [{Name}] [{Program}] with arguments [{Arguments}]", nameof(TryLaunchProgram), program, arguments);
			return false;
		}
	}

	public static string? GetProcessName(Process process)
	{
		try
		{
			var processDescription = process.MainModule?.FileVersionInfo.FileDescription;
			if (string.IsNullOrEmpty(processDescription))
				return Path.GetFileName(process.MainModule?.FileVersionInfo.FileName);

			return processDescription;
		}
		catch (Exception)
		{
			var fileName = NativeMethods.Processes.GetProcessFileName(process);
			if (string.IsNullOrEmpty(fileName))
				return null;

			return Path.GetFileName(fileName);
		}
	}

}