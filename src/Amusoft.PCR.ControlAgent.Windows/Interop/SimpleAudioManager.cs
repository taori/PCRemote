using Amusoft.PCR.ControlAgent.Shared;
using NLog;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System;
using Amusoft.PCR.ControlAgent.Windows.Helpers;
using NAudio.CoreAudioApi;

namespace Amusoft.PCR.ControlAgent.Windows.Interop;


public static class SimpleAudioManager
{
	private static readonly Logger Log = LogManager.GetLogger(nameof(SimpleAudioManager));

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

	public static bool TryGetAudioFeeds(out List<AudioFeedResponseItem> feeds)
	{
		var sessionManager = GetMultiMediaSessionManager(DataFlow.Render);
		feeds = new List<AudioFeedResponseItem>();
		try
		{
			sessionManager.RefreshSessions();
			for (int sessionIndex = 0; sessionIndex < sessionManager.Sessions.Count; sessionIndex++)
			{
				var session = sessionManager.Sessions[sessionIndex];
				if (session.IsSystemSoundsSession)
					continue;

				var processName = TryGetProcessName(session);
				feeds.Add(new AudioFeedResponseItem()
				{
					Id = session.GetSessionIdentifier,
					Name = processName,
					Volume = session.SimpleAudioVolume.Volume * 100,
					Muted = session.SimpleAudioVolume.Mute
				});
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
			sessionManager.Dispose();
		}
	}

	private static string TryGetProcessName(AudioSessionControl session)
	{
		var processById = Process.GetProcessById((int)session.GetProcessID);
		return ProcessHelper.GetProcessName(processById);
	}

	public static bool GetMasterVolumeMute()
	{
		var sm = GetAudioMultiMediaEndpoint(DataFlow.Render, Role.Multimedia);
		try
		{
			return sm.AudioEndpointVolume.Mute;
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

	public static bool SetMasterVolumeMute(bool value)
	{
		var sm = GetAudioMultiMediaEndpoint(DataFlow.Render, Role.Multimedia);
		try
		{
			sm.AudioEndpointVolume.Mute = value;
			return value;
		}
		catch (Exception e)
		{
			Log.Error(e);
			return !value;
		}
		finally
		{
			sm.Dispose();
		}
	}

	public static float GetMasterVolume()
	{
		var sm = GetAudioMultiMediaEndpoint(DataFlow.Render, Role.Multimedia);
		try
		{
			return sm.AudioEndpointVolume.MasterVolumeLevelScalar * 100;
		}
		catch (Exception e)
		{
			Log.Error(e);
			return -1;
		}
		finally
		{
			sm.Dispose();
		}
	}

	public static void SetMasterVolume(float newVolume)
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
		}
		catch (Exception e)
		{
			Log.Error(e);
		}
		finally
		{
			sm.Dispose();
		}
	}

	public static bool TryUpdateFeed(AudioFeedResponseItem requestItem)
	{
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

				if (session.GetSessionIdentifier.Equals(requestItem.Id))
				{
					Log.Trace("Found session which matches requested session - Updating");

					var fixedValue = requestItem.Volume <= 0f ? 0f : requestItem.Volume / 100;
					session.SimpleAudioVolume.Mute = requestItem.Muted;
					session.SimpleAudioVolume.Volume = fixedValue;
					foundSession = true;

					Log.Debug("SimpleAudioVolume for {Name} updated", requestItem.Name);
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

public class AudioFeed
{
	public string Name { get; set; }
	public bool Muted { get; set; }
	public float Volume { get; set; }
}