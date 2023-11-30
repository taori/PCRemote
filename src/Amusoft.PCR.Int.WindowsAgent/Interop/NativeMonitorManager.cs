using System.Runtime.InteropServices;
using NLog;

namespace Amusoft.PCR.Int.WindowsAgent.Interop;


public partial class NativeMonitorManager : IDisposable
{
	private static readonly Logger Log = LogManager.GetCurrentClassLogger();

	[DllImport("dxva2.dll", EntryPoint = "GetNumberOfPhysicalMonitorsFromHMONITOR")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, ref uint pdwNumberOfPhysicalMonitors);

	[DllImport("dxva2.dll", EntryPoint = "GetPhysicalMonitorsFromHMONITOR")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

	[DllImport("dxva2.dll", EntryPoint = "GetMonitorBrightness")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool GetMonitorBrightness(IntPtr handle, ref uint minimumBrightness, ref uint currentBrightness, ref uint maxBrightness);

	[DllImport("dxva2.dll", EntryPoint = "SetMonitorBrightness")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool SetMonitorBrightness(IntPtr handle, uint newBrightness);

	[DllImport("dxva2.dll", EntryPoint = "DestroyPhysicalMonitor")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool DestroyPhysicalMonitor(IntPtr hMonitor);

	[DllImport("dxva2.dll", EntryPoint = "DestroyPhysicalMonitors")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool DestroyPhysicalMonitors(uint dwPhysicalMonitorArraySize, [In] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

	[DllImport("user32.dll")]
	private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumMonitorsDelegate lpfnEnum, IntPtr dwData);

	delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);

	private readonly List<MonitorInfo> _monitors = new();

	public IReadOnlyCollection<MonitorInfo> Monitors => _monitors;

	public NativeMonitorManager()
	{
		_monitors.AddRange(GetMonitorsInternal());
	}

	~NativeMonitorManager()
	{
		Dispose();
	}

	public void SetBrightness(MonitorInfo monitorInfo, uint value)
	{
		SetMonitorBrightness(monitorInfo.PhysicalMonitorHandle, Math.Clamp(value, monitorInfo.MinValue, monitorInfo.MaxValue));
		UpdateMonitorData(monitorInfo);
	}

	public void SetBrightness(uint value)
	{
		foreach (var monitor in _monitors)
		{
			SetBrightness(monitor, value);
		}
	}

	private void SetBrightnessInternal(uint brightness, bool refreshMonitorsIfNeeded = true)
	{
		if (_monitors is {Count: 0})
			return;

		var isSomeFail = false;
		foreach (var monitor in _monitors)
		{
			var currentValue = (monitor.MaxValue - monitor.MinValue) * brightness / 100 + monitor.MinValue;
			if (SetMonitorBrightness(monitor.PhysicalMonitorHandle, currentValue))
			{
				monitor.CurrentValue = currentValue;
			}
			else if (refreshMonitorsIfNeeded)
			{
				isSomeFail = true;
				break;
			}
		}

		if (refreshMonitorsIfNeeded && (isSomeFail || _monitors.Count == 0))
		{
			SetBrightnessInternal(brightness, false);
		}
	}

	public double GetAverageBrightness()
	{
		var monitors = _monitors.Select(d => d.CurrentValue).ToArray();
		return monitors.Length > 0
			? monitors.Average(d => d)
			: 0;
	}

	private static List<MonitorInfo> GetMonitorsInternal()
	{
		var monitors = new List<MonitorInfo>();
		EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData) =>
		{
			uint physicalMonitorsCount = 0;
			if (!GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref physicalMonitorsCount))
			{
				// Cannot get monitor count
				return true;
			}

			var physicalMonitors = new PHYSICAL_MONITOR[physicalMonitorsCount];
			if (!GetPhysicalMonitorsFromHMONITOR(hMonitor, physicalMonitorsCount, physicalMonitors))
			{
				// Cannot get physical monitor handle
				return true;
			}

			foreach (var physicalMonitor in physicalMonitors)
			{
				uint minValue = 0, currentValue = 0, maxValue = 0;
				if (!GetMonitorBrightness(physicalMonitor.hPhysicalMonitor, ref minValue, ref currentValue, ref maxValue))
				{
					DestroyPhysicalMonitor(physicalMonitor.hPhysicalMonitor);
					continue;
				}

				Log.Debug("Physical monitor handle: {Handle}", physicalMonitor.hPhysicalMonitor);

				monitors.Add(new MonitorInfo
				{
					Description = physicalMonitor.szPhysicalMonitorDescription,
					PhysicalMonitorHandle = physicalMonitor.hPhysicalMonitor,
					MinValue = minValue,
					CurrentValue = currentValue,
					MaxValue = maxValue,
				});
			}

			return true;
		}, IntPtr.Zero);

		return monitors;
	}

	private void UpdateMonitorData(MonitorInfo info)
	{
		uint minValue = 0, currentValue = 0, maxValue = 0;
		if (GetMonitorBrightness(info.PhysicalMonitorHandle, ref minValue, ref currentValue, ref maxValue))
		{
			info.CurrentValue = currentValue;
			info.MinValue = minValue;
			info.MaxValue = maxValue;
		}
	}

	public void Dispose()
	{
		Log.Debug("Disposing {Name}", nameof(NativeMonitorManager));
		DisposeMonitors(_monitors);
		GC.SuppressFinalize(this);
	}

	private static void DisposeMonitors(IEnumerable<MonitorInfo> monitors)
	{
		var monitorArray = monitors.Select(m => new PHYSICAL_MONITOR { hPhysicalMonitor = m.PhysicalMonitorHandle }).ToArray();
		if (monitorArray.Length > 0)
			DestroyPhysicalMonitors((uint)monitorArray.Length, monitorArray);
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct PHYSICAL_MONITOR
	{
		public IntPtr hPhysicalMonitor;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string szPhysicalMonitorDescription;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Rect
	{
		public int left;
		public int top;
		public int right;
		public int bottom;
	}

	public class MonitorInfo
	{
		public required string Description { get; set; }
		public required uint MinValue { get; set; }
		public required uint MaxValue { get; set; }
		public required IntPtr PhysicalMonitorHandle { get; set; }
		public required uint CurrentValue { get; set; }
	}
}