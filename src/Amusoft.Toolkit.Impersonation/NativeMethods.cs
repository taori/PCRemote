using NLog;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Amusoft.Toolkit.Impersonation;

internal static class NativeMethods
{
	private static readonly Logger Log = LogManager.GetLogger(nameof(NativeMethods));

	[DllImport("advapi32.dll", SetLastError = true)]
	internal static extern bool CreateProcessAsUser(
		IntPtr tokenHandle,
		string? applicationName,
		string commandLine,
		ref SecurityAttributes processAttributes,
		ref SecurityAttributes threadAttributes,
		bool inheritHandles,
		uint creationFlags,
		IntPtr environment,
		string? currentDirectory,
		ref StartupInfo startupInfo,
		out ProcessInformation processInformation);

	[DllImport("advapi32.dll", EntryPoint = "DuplicateTokenEx", SetLastError = true)]
	internal static extern bool DuplicateTokenEx(
		IntPtr existingToken,
		uint desiredAccess,
		ref SecurityAttributes threadAttributes,
		int impersonationLevel,
		int tokenType,
		ref IntPtr newToken);

	[DllImport("advapi32.dll", SetLastError = true)]
	internal static extern bool OpenProcessToken(
		IntPtr processHandle,
		uint desiredAccess,
		ref IntPtr tokenHandle);

	[DllImport("userenv.dll", SetLastError = true)]
	internal static extern bool CreateEnvironmentBlock(
		ref IntPtr environment,
		IntPtr tokenHandle,
		bool inherit);

	[DllImport("userenv.dll", SetLastError = true)]
	internal static extern bool DestroyEnvironmentBlock(
		IntPtr environmentHandle);

	[DllImport("kernel32.dll", SetLastError = true)]
	internal static extern bool CloseHandle(
		IntPtr handle);

	private const short SW_SHOW = 5;
	private const uint TOKEN_QUERY = 0x0008;
	private const uint TOKEN_DUPLICATE = 0x0002;
	private const uint TOKEN_ASSIGN_PRIMARY = 0x0001;
	private const int GENERIC_ALL_ACCESS = 0x10000000;
	private const int STARTF_USESHOWWINDOW = 0x00000001;
	private const int STARTF_FORCEONFEEDBACK = 0x00000040;
	private const uint CREATE_UNICODE_ENVIRONMENT = 0x00000400;

	internal static bool LaunchProcessAsUser(string command, IntPtr processToken, IntPtr environmentHandle)
	{
		var result = false;

		var processInformation = new ProcessInformation();
		var processAttributes = new SecurityAttributes();
		var threadAttributes = new SecurityAttributes();
		processAttributes.Length = (uint) Marshal.SizeOf(processAttributes);
		threadAttributes.Length = (uint) Marshal.SizeOf(threadAttributes);

		var startupInfo = new StartupInfo();
		startupInfo.cb = (uint) Marshal.SizeOf(startupInfo);

		//if this member is NULL, the new process inherits the desktop
		//and window station of its parent process. If this member is
		//an empty string, the process does not inherit the desktop and
		//window station of its parent process; instead, the system
		//determines if a new desktop and window station need to be created.
		//If the impersonated user already has a desktop, the system uses the
		//existing desktop.

		startupInfo.lpDesktop = @"WinSta0\Default"; //Modify as needed
		startupInfo.dwFlags = STARTF_USESHOWWINDOW | STARTF_FORCEONFEEDBACK;
		startupInfo.wShowWindow = SW_SHOW;
		//Set other si properties as required.

		result = CreateProcessAsUser(
			processToken,
			null,
			command,
			ref processAttributes,
			ref threadAttributes,
			false,
			CREATE_UNICODE_ENVIRONMENT,
			environmentHandle,
			null,
			ref startupInfo,
			out processInformation);

		if (!result)
		{
			Log.Error("Failed to create process as user {Error}", Marshal.GetLastWin32Error());
		}

		return result;
	}

	internal static IntPtr GetPrimaryToken(int processId)
	{
		var token = IntPtr.Zero;
		var primaryToken = IntPtr.Zero;
		var retVal = false;
		Process p;

		try
		{
			p = Process.GetProcessById(processId);
		}
		catch (ArgumentException ex)
		{
			Log.Error("Process id {{id}} is not available. {{exception}}", processId, ex);
			throw;
		}

		//Gets impersonation token
		if (!OpenProcessToken(p.Handle, TOKEN_DUPLICATE, ref token))
		{
			Log.Error("{Name} {Error}", nameof(OpenProcessToken), Marshal.GetLastWin32Error());
			return primaryToken;
		}

		var sa = new SecurityAttributes();
		sa.Length = (uint) Marshal.SizeOf(sa);

		//Convert the impersonation token into Primary token
		retVal = DuplicateTokenEx(
			token,
			TOKEN_ASSIGN_PRIMARY | TOKEN_DUPLICATE | TOKEN_QUERY,
			ref sa,
			(int) SecurityImpersonationLevel.SecurityIdentification,
			(int) TokenType.TokenPrimary,
			ref primaryToken);

		//Close the Token that was previously opened.
		CloseHandle(token);
		if (retVal == false)
		{
			Log.Error("{Name} {Error}", nameof(DuplicateTokenEx), Marshal.GetLastWin32Error());
		}

		//We'll Close this token after it is used.
		return primaryToken;
	}

	internal static IntPtr GetEnvironmentBlock(IntPtr token)
	{
		var envBlock = IntPtr.Zero;
		var retVal = CreateEnvironmentBlock(ref envBlock, token, false);
		if (retVal == false)
		{
			//Environment Block, things like common paths to My Documents etc.
			//Will not be created if "false"
			//It should not adversley affect CreateProcessAsUser.

			Log.Error("Failed to create environment block {Error}", Marshal.GetLastWin32Error());
		}

		return envBlock;
	}
}