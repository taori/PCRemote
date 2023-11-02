using System.Net;
using Amusoft.PCR.Application.Features.DesktopIntegration;

namespace Amusoft.PCR.App.UI.Implementations;

public class DesktopIntegrationServiceFactory : IDesktopIntegrationServiceFactory
{
	public IDesktopIntegrationService Create(string protocol, IPEndPoint endPoint)
	{
		return new DesktopIntegrationService(protocol, endPoint);
	}
}