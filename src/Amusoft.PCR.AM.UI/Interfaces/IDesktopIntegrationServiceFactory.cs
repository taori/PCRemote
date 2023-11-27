using System.Net;

namespace Amusoft.PCR.Application.Features.DesktopIntegration;

public interface IDesktopIntegrationServiceFactory
{
	IDesktopIntegrationService Create(string protocol, IPEndPoint endPoint);
}