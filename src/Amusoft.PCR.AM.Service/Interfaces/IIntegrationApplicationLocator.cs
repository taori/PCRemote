namespace Amusoft.PCR.AM.Service.Interfaces;


public interface IIntegrationApplicationLocator
{
	bool IsOperational();
	bool IsRunning();
	string GetAbsolutePath();
	IEnumerable<int> GetRunningProcessIds();
}

