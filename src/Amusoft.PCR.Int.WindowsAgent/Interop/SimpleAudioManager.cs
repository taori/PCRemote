using System.Diagnostics;
using Amusoft.PCR.Domain.Agent.Entities;
using Amusoft.PCR.Int.IPC;
using NAudio.CoreAudioApi;
using NLog;

namespace Amusoft.PCR.Int.WindowsAgent.Interop;


public static class SimpleAudioManager
{
	private static readonly Logger Log = LogManager.GetCurrentClassLogger();

	private static AudioSessionManager GetMultiMediaSessionManager(DataFlow dataFlow)
	{
		using (var device = GetAudioMultiMediaEndpoint(dataFlow, Role.Multimedia))
		{
			Log.Trace("DefaultDevice: " + device.FriendlyName);
			return device.AudioSessionManager;
		}
	}

	private static MMDevice GetAudioMultiMediaEndpoint(DataFlow dataFlow, Role role)
	{
		using (var enumerator = new MMDeviceEnumerator())
		{
			return enumerator.GetDefaultAudioEndpoint(dataFlow, role);
		}
	}

	public static bool TryGetAudioFeeds(out List<AudioFeedData> feeds)
	{
		var sessionManager = GetMultiMediaSessionManager(DataFlow.Render);
		feeds = new List<AudioFeedData>();
		try
		{
			sessionManager.RefreshSessions();
			for (int sessionIndex = 0; sessionIndex < sessionManager.Sessions.Count; sessionIndex++)
			{
				var session = sessionManager.Sessions[sessionIndex];
				if (session.IsSystemSoundsSession)
					continue;
				
				var processName = TryGetProcessName(session);
				feeds.Add(new AudioFeedData(
					session.GetSessionIdentifier,
					processName,
					session.SimpleAudioVolume.Volume * 100,
					session.SimpleAudioVolume.Mute)
				);
			}

			feeds.Insert(0, new AudioFeedData("master", "Master volume", GetMasterVolume() ?? 100, GetMasterVolumeMute() == true));

			return true;
		}
		catch (Exception e)
		{
			Log.Error(e);
			return false;
		}
		finally
		{
			sessionManager.Dispose();
		}
	}

	private static string? TryGetProcessName(AudioSessionControl session)
	{
		var processById = Process.GetProcessById((int)session.GetProcessID);
		return ProcessHelper.GetProcessName(processById);
	}

	public static bool? GetMasterVolumeMute()
	{
		var sm = GetAudioMultiMediaEndpoint(DataFlow.Render, Role.Multimedia);
		try
		{
			return sm.AudioEndpointVolume.Mute;
		}
		catch (Exception e)
		{
			Log.Error(e);
			return default;
		}
		finally
		{
			sm.Dispose();
		}
	}

	public static bool SetMasterVolumeMute(bool value)
	{
		var sm = GetAudioMultiMediaEndpoint(DataFlow.Render, Role.Multimedia);
		try
		{
			sm.AudioEndpointVolume.Mute = value;
			return true;
		}
		catch (Exception e)
		{
			Log.Error(e);
			return false;
		}
		finally
		{
			sm.Dispose();
		}
	}

	public static float? GetMasterVolume()
	{
		var sm = GetAudioMultiMediaEndpoint(DataFlow.Render, Role.Multimedia);
		try
		{
			return sm.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
		}
		catch (Exception e)
		{
			Log.Error(e);
			return default;
		}
		finally
		{
			sm.Dispose();
		}
	}

	public static bool SetMasterVolume(float newVolume)
	{
		var sm = GetAudioMultiMediaEndpoint(DataFlow.Render, Role.Multimedia);
		try
		{
			if (newVolume <= 0f)
			{
				sm.AudioEndpointVolume.MasterVolumeLevelScalar = 0;
			}
			else
			{
				sm.AudioEndpointVolume.MasterVolumeLevelScalar = newVolume / 100;
			}

			return true;
		}
		catch (Exception e)
		{
			Log.Error(e);
			return false;
		}
		finally
		{
			sm.Dispose();
		}
	}

	public static bool TryUpdateFeed(AudioFeedData dataItem)
	{
		if (dataItem.Id == "master")
		{
			return SetMasterVolume(dataItem.Volume)
			       && SetMasterVolumeMute(dataItem.Muted);

		}

		var sessionManager = GetMultiMediaSessionManager(DataFlow.Render);
		var foundSession = false;
		try
		{
			sessionManager.RefreshSessions();
			Log.Trace("Found {Count} running audio sessions", sessionManager.Sessions.Count);
			for (int sessionIndex = 0; sessionIndex < sessionManager.Sessions.Count; sessionIndex++)
			{
				var session = sessionManager.Sessions[sessionIndex];
				if (session.IsSystemSoundsSession)
					continue;

				if (session.GetSessionIdentifier.Equals(dataItem.Id))
				{
					Log.Trace("Found session which matches requested session - Updating");

					var fixedValue = dataItem.Volume <= 0f ? 0f : dataItem.Volume / 100;
					session.SimpleAudioVolume.Mute = dataItem.Muted;
					session.SimpleAudioVolume.Volume = fixedValue;
					foundSession = true;

					Log.Debug("SimpleAudioVolume for {Name} updated", dataItem.Name);
					break;
				}
			}

			return foundSession;
		}
		catch (Exception e)
		{
			Log.Error(e);
			return false;
		}
		finally
		{
			sessionManager.Dispose();
		}
	}
}