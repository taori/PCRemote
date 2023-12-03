using Amusoft.PCR.Domain.Shared.Entities;
using Amusoft.PCR.Domain.Shared.ValueTypes;

namespace Amusoft.PCR.Int.IPC.Extensions;

public static class DomainIpcBridgeExtensions
{
	public static IEnumerable<GetMonitorBrightnessResponseItem> ToGrpcItems(this IEnumerable<MonitorData> source)
	{
		foreach (var s in source)
		{
			yield return new GetMonitorBrightnessResponseItem()
			{
				Id = s.Id,
				Name = s.Description,
				Current = (int)s.Current,
				Min = (int)s.Min,
				Max = (int)s.Max,
			};
		}
	}
	
	public static IEnumerable<AudioFeedResponseItem> ToGrpcItems(this IEnumerable<AudioFeedData> source)
	{
		foreach (var s in source)
		{
			yield return new AudioFeedResponseItem()
			{
				Id = s.Id,
				Name = s.Name,
				Muted = s.Muted,
				Volume = s.Volume,
			};
		}
	}
	
	public static AudioFeedResponseItem ToGrpcItem(this AudioFeedData source)
	{
		return new AudioFeedResponseItem()
		{
			Id = source.Id,
			Name = source.Name,
			Muted = source.Muted,
			Volume = source.Volume,
		};
	}
	
	public static IEnumerable<ProcessListResponseItem> ToGrpcItems(this IEnumerable<ProcessData> source)
	{
		foreach (var s in source)
		{
			yield return new ProcessListResponseItem()
			{
				ProcessName = s.ProcessName,
				ProcessId = s.ProcessId,
				MainWindowTitle = s.MainWindowTitle,
				CpuUsage = s.CpuUsage,
			};
		}
	}
	
	public static MonitorData ToDomainItem(this GetMonitorBrightnessResponseItem source)
	{
		return new MonitorData(source.Id, source.Name, (uint)source.Current, (uint)source.Min, (uint)source.Max);
	}
	
	public static ProcessData ToDomainItem(this ProcessListResponseItem source)
	{
		return new ProcessData(source.ProcessId, source.ProcessName, source.MainWindowTitle, source.CpuUsage);
	}
	
	public static AudioFeedData ToDomainItem(this AudioFeedResponseItem source)
	{
		return new AudioFeedData(source.Id, source.Name, source.Volume, source.Muted);
	}
	
	public static MediaKeyCode ToDomainType(this SendMediaKeysRequest.Types.MediaKeyCode source)
	{
		return source switch
		{
			SendMediaKeysRequest.Types.MediaKeyCode.NextTrack => MediaKeyCode.NextTrack,
			SendMediaKeysRequest.Types.MediaKeyCode.PreviousTrack => MediaKeyCode.PreviousTrack,
			SendMediaKeysRequest.Types.MediaKeyCode.PlayPause => MediaKeyCode.PlayPause,
			_ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
		};
	}
	
	public static SendMediaKeysRequest.Types.MediaKeyCode ToGrpcType(this MediaKeyCode source)
	{
		return source switch
		{
			MediaKeyCode.NextTrack => SendMediaKeysRequest.Types.MediaKeyCode.NextTrack,
			MediaKeyCode.PreviousTrack => SendMediaKeysRequest.Types.MediaKeyCode.PreviousTrack,
			MediaKeyCode.PlayPause => SendMediaKeysRequest.Types.MediaKeyCode.PlayPause,
			_ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
		};
	}
}