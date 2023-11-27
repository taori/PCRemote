using System.Diagnostics;

namespace Amusoft.Toolkit.Impersonation;

// https://social.msdn.microsoft.com/Forums/vstudio/en-US/0c0ca087-5e7b-4046-93cb-c7b3e48d0dfb/how-run-client-application-as-a-windows-service-in-c?forum=csharpgeneral

/// <summary>
/// This class serves the purpose of executing a process in the user context of another process
/// </summary>
public class ProcessImpersonation
{
	/// <summary>
	/// Executes a process using the process id of the first explorer process it finds.
	/// </summary>
	/// <param name="command">command to execute</param>
	/// <returns></returns>
	public static bool Launch(string command) => Launch(command, int.MinValue);

	/// <summary>
	/// Executes a process using the process id which is specified
	/// </summary>
	/// <param name="command">command to execute</param>
	/// <param name="processId">id of process which is used to impersonate the command</param>
	/// <returns></returns>
	public static bool Launch(string command, int processId)
	{
		bool success = false;

		//Either specify the processID explicitly
		//Or try to get it from a process owned by the user.
		//In this case assuming there is only one explorer.exe

		if (processId < 0)
			processId = GetExplorerProcessId();

		if (processId > 1)
		{
			var processToken = NativeMethods.GetPrimaryToken(processId);
			if (processToken != IntPtr.Zero)
			{
				var environmentHandle = NativeMethods.GetEnvironmentBlock(processToken);
				success = NativeMethods.LaunchProcessAsUser(command, processToken, environmentHandle);
				if (environmentHandle != IntPtr.Zero)
					NativeMethods.DestroyEnvironmentBlock(environmentHandle);

				NativeMethods.CloseHandle(processToken);
			}
		}

		return success;
	}

	private static int GetExplorerProcessId()
	{
		return Process.GetProcessesByName("explorer").FirstOrDefault()?.Id ?? int.MinValue;
	}
}