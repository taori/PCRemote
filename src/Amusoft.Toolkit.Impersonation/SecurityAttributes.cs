using System.Runtime.InteropServices;

namespace Amusoft.Toolkit.Impersonation;

[StructLayout(LayoutKind.Sequential)]
internal struct SecurityAttributes
{
	public uint Length;
	public IntPtr SecurityDescriptor;
	public bool InheritHandle;
}