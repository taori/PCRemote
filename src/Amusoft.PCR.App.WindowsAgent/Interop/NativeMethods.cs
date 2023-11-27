using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using NLog;

namespace Amusoft.PCR.App.WindowsAgent.Interop;


[Flags]
public enum MouseEventFlags
{
	LeftDown = 0x00000002,
	LeftUp = 0x00000004,
	MiddleDown = 0x00000020,
	MiddleUp = 0x00000040,
	Move = 0x00000001,
	Absolute = 0x00008000,
	RightDown = 0x00000008,
	RightUp = 0x00000010
}

[StructLayout(LayoutKind.Sequential)]
public struct MousePoint
{
	public int X;
	public int Y;

	public MousePoint(int x, int y)
	{
		X = x;
		Y = y;
	}
}

internal class NativeMethods
{
	private static readonly Logger Log = LogManager.GetCurrentClassLogger();

	[DllImport("user32.dll")]
	static extern bool SetForegroundWindow(IntPtr hWnd);

	[DllImport("user32.dll")]
	public static extern void LockWorkStation();

	/// <summary>
	/// See https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.sendkeys?redirectedfrom=MSDN&view=net-5.0
	/// </summary>
	/// <param name="keys"></param>
	public static void SendKeys(string keys)
	{
		System.Windows.Forms.SendKeys.SendWait(keys);
	}

	private static unsafe IntPtr GetProcessPointer()
	{
		var p = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
		var intPtr = new IntPtr(p.ToPointer());
		return intPtr;
	}

	public static bool SetForegroundWindow(int processId)
	{
		try
		{
			var matchProcess = Process.GetProcessById(processId);
			return SetForegroundWindow(matchProcess.MainWindowHandle);
		}
		catch (Exception e)
		{
			Log.Error(e, "An error occured while calling {Method}", nameof(SetForegroundWindow));
			return false;
		}
	}

	public enum MediaKeyCode : byte
	{
		NextTrack = 0xb0,
		PreviousTrack = 0xb3,
		PlayPause = 0xb1
	}

	[DllImport("user32.dll", SetLastError = true)]
	public static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);
	public const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
	public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag

	public static void PressMediaKey(MediaKeyCode code)
	{
		keybd_event((byte)code, 0, KEYEVENTF_EXTENDEDKEY, IntPtr.Zero);
		keybd_event((byte)code, 0, KEYEVENTF_KEYUP, IntPtr.Zero);
	}

	public static class Mouse
	{
		[DllImport("user32.dll")]
		static extern void mouse_event(Int32 dwFlags, Int32 dx, Int32 dy, Int32 dwData, UIntPtr dwExtraInfo);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetCursorPos(out MousePoint lpMousePoint);
		public static MousePoint GetCursorPosition()
		{
			MousePoint currentMousePoint;
			var gotPoint = GetCursorPos(out currentMousePoint);
			if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
			return currentMousePoint;
		}

		public static void Move(int x, int y)
		{
			mouse_event((int)MouseEventFlags.Move, x, y, 0, UIntPtr.Zero);
		}
		public static void MouseEvent(MouseEventFlags value)
		{
			MousePoint position = GetCursorPosition();
			mouse_event
				((int)value,
					position.X,
					position.Y,
					0,
					UIntPtr.Zero)
				;
		}

		public static void ClickLeft()
		{
			MouseEvent(MouseEventFlags.LeftDown);
			Thread.Sleep(20);
			MouseEvent(MouseEventFlags.LeftUp);
		}

		public static void ClickRight()
		{
			MouseEvent(MouseEventFlags.RightDown);
			Thread.Sleep(20);
			MouseEvent(MouseEventFlags.RightUp);
		}
	}

	public static class Monitor
	{
		[DllImport("user32.dll")]
		static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		private const int WmSyscommand = 0x0112;
		private const int ScMonitorpower = 0xF170;
		private const int MonitorShutoff = 0x0002;
		private const int Broadcast = 0xffff;

		public static void Off()
		{
			SendMessage((IntPtr)Broadcast, WmSyscommand, (IntPtr)ScMonitorpower, (IntPtr)MonitorShutoff);
		}

		public static void On()
		{
			Mouse.Move(0, 1);
			Thread.Sleep(40);
			Mouse.Move(0, -1);
		}
	}

	public static class Processes
	{
		[Flags]
		private enum ProcessAccessFlags : uint
		{
			QueryLimitedInformation = 0x00001000
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool QueryFullProcessImageName(
			[In] IntPtr hProcess,
			[In] int dwFlags,
			[Out] StringBuilder lpExeName,
			ref int lpdwSize);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr OpenProcess(
			ProcessAccessFlags processAccess,
			bool bInheritHandle,
			int processId);

		public static string GetProcessFileName(Process process)
		{
			int capacity = 2000;
			var builder = new StringBuilder(capacity);
			var ptr = OpenProcess(ProcessAccessFlags.QueryLimitedInformation, false, process.Id);
			if (!QueryFullProcessImageName(ptr, 0, builder, ref capacity))
			{
				return string.Empty;
			}

			return builder.ToString();
		}
	}
}