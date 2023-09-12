using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;
using Amusoft.PCR.Installer.Custom.Extensions;

namespace Amusoft.PCR.Installer.Custom
{
	public class CustomActions
	{
		[CustomAction]
		public static ActionResult UpdateAppSettings(Session session)
		{
			SetBreakpoint(nameof(UpdateAppSettings));

			session.Log("UpdateAppSettings Enter");
			
			if (!session.CustomActionData.TryGetValue("WebApplicationDirectory", out var serverRootDirectory))
				return ActionResult.Failure;

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

			var productionConfigFile = Path.Combine(serverRootDirectory, "appsettings.Production.json");
			var originalConfigFile = Path.Combine(serverRootDirectory, "appsettings.json");

			if (isUninstall)
			{
				session.Log($"Deleting file {productionConfigFile}: {File.Exists(productionConfigFile)}");
				if (File.Exists(productionConfigFile))
					File.Delete(productionConfigFile);

				session.Log($"Deleting file {originalConfigFile}: {File.Exists(originalConfigFile)}");
				if (File.Exists(originalConfigFile))
					File.Delete(originalConfigFile);

				return ActionResult.Success;
			}
			
			if (!session.TryGetIntOrDefault("Port", 0, out var port))
				return ActionResult.Failure;

			if (isInstall)
			{
				try
				{
					session.Log($"Deleting file {productionConfigFile}: {File.Exists(productionConfigFile)}");
					if (File.Exists(productionConfigFile))
						File.Delete(productionConfigFile);
					
					session.Log($"Moving file from {originalConfigFile} to {productionConfigFile}");
					File.Move(originalConfigFile, productionConfigFile);

					var content = File.ReadAllText(productionConfigFile, Encoding.UTF8);
					var stringBuilder = new StringBuilder(content);
					session.Log($"Replacing port 5001 with {port}");
					stringBuilder.Replace("5001", port.ToString());
					session.Log("Updating production file");
					File.WriteAllText(productionConfigFile, stringBuilder.ToString());
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
		public static ActionResult StopService(Session session)
		{
			SetBreakpoint(nameof(StopService));

			// required to shut down system processes
			if (!session.CustomActionData.TryGetValue("ServiceName", out var serviceName))
				return ActionResult.Failure;

			try
			{
				var controller = new ServiceController(serviceName);
				controller.Stop();
				controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));

				return ActionResult.Success;
			}
			catch (Exception e)
			{
				session.Log($"Service failed to start: {e.ToString()}");
				return ActionResult.Failure;
			}
		}

		private static bool TryGetPropertyValue(Session session, string key, out string value)
		{
			if (session.CustomActionData.TryGetValue(key, out value))
				return true;

			value = session[key];
			return value != null;
		}

		[CustomAction]
		public static ActionResult VerifyConfiguration(Session session)
		{
			SetBreakpoint(nameof(VerifyConfiguration));

			session.Log("Trying to read port");

			if (!TryGetPropertyValue(session, "CUSTOM_PORT", out var portValue))
			{
				session.Log($"Not available in {nameof(session.CustomActionData)}");
				return ActionResult.Failure;
			}

			try
			{
				session.Log($"Trying to trim CUSTOM_PORT");
				session["CUSTOM_PORT"] = portValue.Trim();
				return ActionResult.Success;
			}
			catch (Exception e)
			{
				session.Log($"Service failed to start: {e.ToString()}");
				return ActionResult.Failure;
			}
		}

		private static bool InvokeServiceCommandUtil(Session session, string command)
		{
			try
			{
				var process = new Process();
				process.StartInfo = new ProcessStartInfo("sc.exe", command);
				process.StartInfo.Verb = "runas";
				process.StartInfo.UseShellExecute = true;
				process.Start();
				return process.WaitForExit((int)TimeSpan.FromSeconds(60).TotalMilliseconds);
			}
			catch (Exception e)
			{
				session.Log(e.ToString());
				return false;
			}
		}

		[CustomAction]
		public static ActionResult UninstallService(Session session)
		{
			SetBreakpoint(nameof(UninstallService));

			// required to shut down system processes
			if (!session.CustomActionData.TryGetValue("ServiceName", out var serviceName))
				return ActionResult.Failure;

			try
			{
				return InvokeServiceCommandUtil(session, $"delete \"{serviceName}\"")
					? ActionResult.Success
					: ActionResult.Failure;
			}
			catch (Exception e)
			{
				session.Log($"Service failed to start: {e.ToString()}");
				return ActionResult.Failure;
			}
		}

		[CustomAction]
		public static ActionResult InstallService(Session session)
		{
			SetBreakpoint(nameof(InstallService));

			if (!session.CustomActionData.TryGetValue("DisplayName", out var displayName))
				return ActionResult.Failure;
			if (!session.CustomActionData.TryGetValue("ServiceName", out var serviceName))
				return ActionResult.Failure;
			if (!session.CustomActionData.TryGetValue("ServiceDescription", out var description))
				return ActionResult.Failure;
			if (!session.CustomActionData.TryGetValue("ResetConfiguration", out var resetConfiguration))
				return ActionResult.Failure;
			if (!session.CustomActionData.TryGetValue("ServiceExePath", out var exePath))
				return ActionResult.Failure;

			if (!InvokeServiceCommandUtil(session, $"create \"{serviceName}\" binPath= \"{exePath}\" DisplayName= \"{displayName}\" start= auto"))
			{
				session.Log("Creation failed.");
				return ActionResult.Failure;
			}

			if (!InvokeServiceCommandUtil(session, $"description \"{serviceName}\" \"{description}\""))
			{
				session.Log("Setting description failed.");
				return ActionResult.Failure;
			}

			var fullFailureReset = TimeSpan.FromHours(24).TotalSeconds;
			if (!InvokeServiceCommandUtil(session, $"failure \"{serviceName}\" reset= {fullFailureReset} actions= {resetConfiguration}"))
			{
				session.Log("Setting failure configuration failed.");
				return ActionResult.Failure;
			}

			return ActionResult.Success;
		}

		[CustomAction]
		public static ActionResult StartService(Session session)
		{
			SetBreakpoint(nameof(StartService));

			// required to shut down system processes
			if (!session.CustomActionData.TryGetValue("ServiceName", out var serviceName))
				return ActionResult.Failure;

			try
			{
				if (InvokeServiceCommandUtil(session, $"start \"{serviceName}\""))
				{
					var controller = new ServiceController(serviceName);
					controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
				}
				
				return ActionResult.Success;
			}
			catch (Exception e)
			{
				session.Log($"Service failed to start: {e.ToString()}");
				return ActionResult.Failure;
			}
		}

		[Conditional("DEBUG")]
		private static void SetBreakpoint(string name)
		{
			var test = MessageBox.Show($"Waiting for breakpoint ID: {Process.GetCurrentProcess().Id} in {name}");
		}
	}
}
