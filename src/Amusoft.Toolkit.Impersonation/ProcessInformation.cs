using System.Runtime.InteropServices;

namespace Amusoft.Toolkit.Impersonation;

[StructLayout(LayoutKind.Sequential)]
internal struct ProcessInformation
{
	public IntPtr ProcessHandle;
	public IntPtr ThreadHandle;
	public uint ProcessId;
	public uint ThreadId;
}