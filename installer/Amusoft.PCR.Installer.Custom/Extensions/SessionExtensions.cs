using Microsoft.Deployment.WindowsInstaller;

namespace Amusoft.PCR.Installer.Custom.Extensions
{
	public static class SessionExtensions
	{
		public static bool TryGetValueOrDefault(this Session session, string key, string defaultValue, out string returnValue)
		{
			if (!session.CustomActionData.TryGetValue(key, out var keyValue))
			{
				session.Log($"Reading {nameof(session.CustomActionData)} -> {key} failed - returning {defaultValue}");
				returnValue = defaultValue;
				return false;
			}

			if (string.IsNullOrEmpty(keyValue.Trim()))
			{
				session.Log($"Reading {nameof(session.CustomActionData)} -> {key} was empty after trimming - returning {defaultValue}");
				returnValue = defaultValue;
				return false;
			}
			else
			{
				returnValue = keyValue;
				return true;
			}
		}

		public static bool TryGetIntOrDefault(this Session session, string key, int defaultValue, out int resultValue)
		{
			if (!session.TryGetValueOrDefault(key, "0", out var stringValue))
			{
				resultValue = defaultValue;
				return false;
			}
			if (int.TryParse(stringValue, out var parsed))
			{
				session.Log($"Reading {nameof(session.CustomActionData)} -> {key} success - returning {parsed}");
				resultValue = parsed;
				return true;
			}
			else
			{
				session.Log($"Reading {nameof(session.CustomActionData)} -> {key} parsing error - returning {defaultValue}");
				resultValue = defaultValue;
				return false;
			}
		}
	}
}