using System.Diagnostics;

namespace Amusoft.PCR.App.WindowsAgent.Interop;

public static class MachineStateHelper
{
	public static bool TryAbortShutDown()
	{
		try
		{
			using (var process = Process.Start("shutdown", "/a"))
			{
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.UseShellExecute = true;
				process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			}

			return true;
		}
		catch (Exception e)
		{
			Debug.WriteLine(e);
			return false;
		}
	}

	public static bool TryShutDownDelayed(TimeSpan timespan, bool force)
	{
		try
		{
			string forcedAppend = force ? " /f" : string.Empty;
			using (var process = Process.Start("shutdown", $"/s /t {timespan.TotalSeconds}{forcedAppend} /d p:0:0 /c \"Shutdown requested through PC Remote Controller 3.\""))
			{
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.UseShellExecute = true;
				process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			}

			return true;
		}
		catch (Exception e)
		{
			Debug.WriteLine(e);
			return false;
		}
	}

	public static bool TryRestart(TimeSpan timespan, bool force)
	{
		try
		{
			string forcedAppend = force ? " /f" : string.Empty;
			using (var process = Process.Start("shutdown", $"/r /t {timespan.TotalSeconds} {forcedAppend} /d p:4:1 /c \"Restart requested through PC Remote Controller 2.\""))
			{
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.UseShellExecute = true;
				process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			}

			return true;
		}
		catch (Exception e)
		{
			Debug.WriteLine(e);
			return false;
		}
	}

	public static bool TryHibernate()
	{
		try
		{
			System.Windows.Forms.Application.SetSuspendState(PowerState.Hibernate, false, false);
			return true;
		}
		catch (Exception e)
		{
			Debug.WriteLine(e);
			return false;
		}
	}
}