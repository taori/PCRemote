using Amusoft.PCR.AM.Agent.Interfaces;

namespace Amusoft.PCR.Int.WindowsAgent.Dependencies;

public class ApplicationController : IApplicationController
{
	public void Shutdown()
	{
		System.Windows.Application.Current.Shutdown();
	}
}