namespace Amusoft.PCR.AM.Agent.Interfaces;

public interface IApplicationHost
{
	void ExecuteStartup();
	void ExecuteShutdown();
}