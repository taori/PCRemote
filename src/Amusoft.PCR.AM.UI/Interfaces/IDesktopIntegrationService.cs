using Amusoft.PCR.AM.Shared.Services;

namespace Amusoft.PCR.Application.Features.DesktopIntegration;

public interface IDesktopIntegrationService
{
	IDesktopClientMethods DesktopClient { get; }
}