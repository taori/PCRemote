using Amusoft.PCR.Application.Services;

namespace Amusoft.PCR.Application.Features.DesktopIntegration;

public interface IDesktopIntegrationService
{
	IDesktopClientMethods DesktopClient { get; }
}