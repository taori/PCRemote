using System.Diagnostics;
using System.Reflection;
using Amusoft.PCR.Domain.AgentSettings;

namespace Amusoft.PCR.Domain.Services;


public interface IIntegrationApplicationLocator
{
	bool IsOperational();
	bool IsRunning();
	string GetAbsolutePath();
	IEnumerable<(int processId, string path)> GetIntegrationProcesses();
}

