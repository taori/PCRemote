using Amusoft.PCR.Domain.AgentSettings;
using Amusoft.PCR.Domain.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Reflection;

namespace Amusoft.PCR.Application.Features.DesktopIntegration;

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

	private string GetInferredAbsolutePath(string exePath)
	{
		var executingAssemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		var originUri = new Uri(executingAssemblyDirectory + Path.DirectorySeparatorChar, UriKind.Absolute);
		var relativePortion = new Uri(exePath, UriKind.Relative);
		var combinedUri = new Uri(originUri, relativePortion);

		var resultPath = combinedUri.LocalPath;
		_logger.LogTrace("Combined Uri {Path} from {OriginPath} and {RelativePath}", resultPath, executingAssemblyDirectory, exePath);

		return resultPath;
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
			var fullPath = GetInferredAbsolutePath(_settings.ExePath!);
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
		return GetIntegrationProcesses().Any();
	}

	public bool IsRunning()
	{
		return GetIntegrationProcesses().Any();
	}

	public string GetAbsolutePath()
	{
		return GetInferredAbsolutePath(_settings.ExePath!);
	}

	public IEnumerable<(int processId, string path)> GetIntegrationProcesses()
	{
		var normalizedFileName = Path.GetFileName(Path.GetFullPath(GetAbsolutePath()));
		var allProcesses = GetProcessExePaths();

		return allProcesses
			.Where(d => Path.GetFileName(Path.GetFullPath(d.fullPath)).Equals(normalizedFileName));
	}

	private IReadOnlyList<(int processId, string fullPath)> GetProcessExePaths()
	{
		var results = new List<(int processId, string fullPath)>();
		foreach (var process in Process.GetProcesses())
		{
			try
			{
				if (process.MainModule?.FileName != null)
				{
					results.Add((process.Id, process.MainModule.FileName));
				}
			}
			catch (Exception)
			{
				// ignored
			}
		}

		return results;
	}
}