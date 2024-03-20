using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Amusoft.PCR.Installer.Custom.Extensions;
using WixToolset.Dtf.WindowsInstaller;

namespace Amusoft.PCR.Installer.Custom
{
	public class CustomActions
	{
		[CustomAction]
		public static ActionResult VerifyConfigurationAction(Session session)
		{
			SetBreakpoint(nameof(VerifyConfigurationAction));

			session.Log("Trying to read port");

			if (!TryGetPropertyValue(session, "CUSTOM_PORT_SERVER", out var serverPort))
			{
				session.Log($"Not available in {nameof(session.CustomActionData)}");
				return ActionResult.Failure;
			}

			if (!TryGetPropertyValue(session, "CUSTOM_PORT_DISCOVERY", out var discoveryPort))
			{
				session.Log($"Not available in {nameof(session.CustomActionData)}");
				return ActionResult.Failure;
			}

			try
			{
				//49,152–65,535
				if (!int.TryParse(serverPort.Trim(), out var sp) || sp > 65535)
				{
					session.Log($"{serverPort} is not a valid (server) port number. Trying staying below 65,535");
					return ActionResult.Failure;
				}
				if (!int.TryParse(discoveryPort.Trim(), out var dp) || dp > 65535)
				{
					session.Log($"{discoveryPort} is not a valid (discovery) port number. Trying staying below 65,535");
					return ActionResult.Failure;
				}
				
				session["CUSTOM_PORT_SERVER"] = serverPort.Trim();
				session["CUSTOM_PORT_DISCOVERY"] = discoveryPort.Trim();
				return ActionResult.Success;
			}
			catch (Exception e)
			{
				session.Log($"Service failed to start: {e.ToString()}");
				return ActionResult.Failure;
			}
		}
		
		[CustomAction]
		public static ActionResult UpdateAppSettings(Session session)
		{
			SetBreakpoint(nameof(UpdateAppSettings));

			session.Log("UpdateAppSettings Enter");

			if (!session.CustomActionData.TryGetValue("WebApplicationDirectory", out var serverRootDirectory))
			{
				session.Log("Failed to access WebApplicationDirectory");
				return ActionResult.Failure;
			}
			if (!session.CustomActionData.TryGetValue("ServerPort", out var serverPort))
			{
				session.Log("Failed to access ServerPort");
				return ActionResult.Failure;
			}
			if (!session.CustomActionData.TryGetValue("DiscoveryPort", out var discoveryPort))
			{
				session.Log("Failed to access DiscoveryPort");
				return ActionResult.Failure;
			}
			if (!session.CustomActionData.TryGetValue("EndpointName", out var endpointName))
			{
				session.Log("Failed to access EndpointName");
				return ActionResult.Failure;
			}

			if (!session.CustomActionData.TryGetValue("PfxPassword", out var pfxPassword))
			{
				session.Log("Failed to access PfxPassword");
				return ActionResult.Failure;
			}

			var oldServerPfxPath = "D:\\\\tmp\\\\site.pfx";
			var oldServerPfxPassword = "test1234";
			var newServerPfxPath = Path.Combine(serverRootDirectory, "server.pfx").Replace("\\", "\\\\");
			var isUpgrade = session.TryGetIntOrDefault("IsUpgrade", 0, out var upgradeValue) && upgradeValue > 0;
			var isInstall = session.TryGetIntOrDefault("IsInstall", 0, out var installValue) && installValue > 0;
			var isChange = session.TryGetIntOrDefault("IsChange", 0, out var changeValue) && changeValue > 0;
			var isUninstall = session.TryGetIntOrDefault("IsUninstall", 0, out var uninstallValue) && uninstallValue > 0;
			var isRepair = session.TryGetIntOrDefault("IsRepair", 0, out var repairValue) && repairValue > 0;

			if (isUpgrade || isChange || isRepair)
			{
				session.Log($"This scenario is not covered by {nameof(UpdateAppSettings)}.");
				return ActionResult.Success;
			}

			var appsettingsFile = Path.Combine(serverRootDirectory, "appsettings.json");

			if (isUninstall)
			{
				session.Log($"Deleting file {appsettingsFile}: {File.Exists(appsettingsFile)}");
				if (File.Exists(appsettingsFile))
					File.Delete(appsettingsFile);

				session.Log($"Deleting file {newServerPfxPath}: {File.Exists(newServerPfxPath)}");
				if (File.Exists(newServerPfxPath))
					File.Delete(newServerPfxPath);

				return ActionResult.Success;
			}

			if (isInstall)
			{
				try
				{
					var content = File.ReadAllText(appsettingsFile, Encoding.UTF8);
					var stringBuilder = new StringBuilder(content);
					session.Log($"Replacing server port 5001 with {serverPort}");
					stringBuilder.Replace("\"Url\": \"https://*:5001\"", $"\"Url\": \"https://*:{serverPort}\"");
					
					session.Log($"Replacing discovery port 50001 with {discoveryPort}");
					stringBuilder.Replace("\"HandshakePort\": 50001,", $"\"HandshakePort\": {discoveryPort},");
					
					session.Log($"Replacing endpoint name \"Endpoint 1 (Prod)\" with {endpointName}");
					stringBuilder.Replace("\"HostAlias\": \"Endpoint 1 (Prod)\"", $"\"HostAlias\": \"{endpointName}\"");

					session.Log($"Replacing {oldServerPfxPath} with {newServerPfxPath}");
					stringBuilder.Replace(oldServerPfxPath, newServerPfxPath);

					session.Log("Deleting server.pfx if it exists");
					if (File.Exists(newServerPfxPath))
						File.Delete(newServerPfxPath);

					session.Log($"Replacing {oldServerPfxPassword} with {pfxPassword}");
					stringBuilder.Replace(oldServerPfxPassword, pfxPassword);
					
					session.Log("Updating production file");
					File.WriteAllText(appsettingsFile, stringBuilder.ToString());
					session.Log("Update complete");
				}
				catch (Exception e)
				{
					session.Log(e.ToString());
					return ActionResult.Failure;
				}
			}
			
			return ActionResult.Success;
		}
		
		[CustomAction]
		public static ActionResult PrintEulaAction(Session session)
		{
			session.Log("PrintEulaAction");

			return ActionResult.Success;
		}

		private static bool TryGetPropertyValue(Session session, string key, out string value)
		{
			if (session.CustomActionData.TryGetValue(key, out value))
				return true;

			value = session[key];
			return value != null;
		}

		[Conditional("DEBUG")]
		private static void SetBreakpoint(string name)
		{
			var test = MessageBox.Show($"Waiting for breakpoint ID: {Process.GetCurrentProcess().Id} in {name}");
		}
	}
}
