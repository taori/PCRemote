using System.Net;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IDesktopIntegrationServiceFactory
{
	IDesktopIntegrationService Create(string protocol, IPEndPoint endPoint);
}