using System.Diagnostics;
using System.IO;
using Amusoft.PCR.Domain.Agent.Entities;
using Amusoft.PCR.Int.IPC;
using NLog;

namespace Amusoft.PCR.Int.WindowsAgent.Interop;

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

	public static bool TryGetProcessList(out List<ProcessData> items)
	{
		items = new List<ProcessData>();
		try
		{
			var processes = Process.GetProcesses();
			foreach (var process in processes)
			{
				items.Add(new ProcessData(process.Id, process.ProcessName, process.MainWindowTitle, 0));
			}
			return true;
		}
		catch (Exception e)
		{
			Log.Error(e, nameof(TryGetProcessList));
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