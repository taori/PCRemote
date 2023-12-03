using Amusoft.PCR.AM.Service.Interfaces;
using Amusoft.Toolkit.Impersonation;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.Service.Services;

internal class ImpersonatedProcessLauncher : IImpersonatedProcessLauncher
{
	private readonly ILogger<ImpersonatedProcessLauncher> _logger;

	public ImpersonatedProcessLauncher(ILogger<ImpersonatedProcessLauncher> logger)
	{
		_logger = logger;
	}
	
	public void Launch(string fullPath)
	{
		_logger.LogDebug("Launching {Path}", fullPath);
		ProcessImpersonation.Launch(fullPath);
	}
}