using System.Diagnostics;
using System.Reflection;
using Amusoft.PCR.AM.Service.Interfaces;
using Amusoft.PCR.Domain.Service.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amusoft.PCR.AM.Service.Services;

public class IntegrationApplicationLocator : IIntegrationApplicationLocator
{
	private readonly ILogger<IntegrationApplicationLocator> _logger;
	private readonly DesktopIntegrationSettings _settings;

	public IntegrationApplicationLocator(ILogger<IntegrationApplicationLocator> logger, IOptions<ApplicationSettings> settings)
	{
		_logger = logger;
		_settings = settings.Value.DesktopIntegration ?? throw new ArgumentNullException(nameof(settings.Value.DesktopIntegration));
		if (_settings.ExePath is null)
			throw new ArgumentNullException(nameof(DesktopIntegrationSettings.ExePath));
	}

	private string GetExePathFromSettings(string exePath)
	{
		var executingAssemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		var originUri = new Uri(executingAssemblyDirectory + Path.DirectorySeparatorChar, UriKind.Absolute);
		var relativePortion = new Uri(exePath, UriKind.Relative);
		var combinedUri = new Uri(originUri, relativePortion);

		var resultPath = combinedUri.LocalPath;
		_logger.LogTrace("Combined Uri {Path} from {OriginPath} and {RelativePath}", resultPath, executingAssemblyDirectory, exePath);

		return resultPath;
	}

	public IEnumerable<int> GetRunningProcessIds()
	{
		var expected = GetApplicationExeName();

		return Process.GetProcesses()
			.Select(d => (d.Id, FileName: GetModuleFileName(d) ?? string.Empty))
			.Where(d => Path.GetFileName(d.FileName).Equals(expected))
			.Select(d => d.Id);
	}

	private static string? GetModuleFileName(Process d)
	{
		try
		{
			return d.MainModule?.FileName;
		}
		catch (Exception)
		{
			// this can throw but we ignore it
			return null;
		}
	}

	private bool IsConfigurationOperational()
	{
		if (string.IsNullOrEmpty(_settings.ExePath))
		{
			_logger.LogError("IntegrationRunner settings ExePath is null or empty");
			return false;
		}

		return true;
	}

	public bool IsOperational()
	{
#if DEBUG
		return IsOperationalInDebug();
#else
			return IsOperationalInRelease();
#endif
	}

	private bool IsOperationalInRelease()
	{
		var result = IsConfigurationOperational();
		if (result)
		{
			var fullPath = GetExePathFromSettings(_settings.ExePath!);
			result = File.Exists(fullPath);

			if (!result)
			{
				_logger.LogCritical("The integration path {Path} does not exist and therefore cannot be launched", fullPath);
			}
		}

		return result;
	}

	private bool IsOperationalInDebug()
	{
		var paths = GetExePathFromBinaryFolderRecursion();
		return paths is not null && File.Exists(paths);
	}

	private string? GetExePathFromBinaryFolderRecursion()
	{
		// D:\GitHub\taori\PCRemote\src\Amusoft.PCR.App.Service\bin\Debug\net7.0\Amusoft.PCR.Application.dll
		var location = Assembly.GetExecutingAssembly().Location;
		var basePath = new Uri(location, UriKind.Absolute);
		var relativeRoute = new Uri("../../../../Amusoft.PCR.App.WindowsAgent/bin/", UriKind.Relative);
		var searchRoot = new Uri(basePath, relativeRoute).LocalPath;
		var fileMatches = Directory.GetFiles(searchRoot, "Amusoft.PCR.App.WindowsAgent.exe", SearchOption.AllDirectories);
		if (fileMatches.FirstOrDefault(d => d.Contains("bin\\Debug", StringComparison.OrdinalIgnoreCase)) is { } match)
			return match;

		return null;
	}

	public bool IsRunning()
	{
		return GetRunningProcessIds().Any();
	}

	public string GetAbsolutePath()
	{
#if DEBUG
		return GetExePathFromBinaryFolderRecursion() ?? throw new Exception("Unable to find a matching exe path for this DEBUG build.");
#else
		return GetExePathFromSettings(_settings.ExePath!);
#endif
	}

	public string GetApplicationExeName()
	{
		return Path.GetFileName(_settings.ExePath!);
	}
}