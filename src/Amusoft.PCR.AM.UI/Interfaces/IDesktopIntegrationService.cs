using Amusoft.PCR.AM.Shared.Interfaces;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IDesktopIntegrationService
{
	IDesktopClientMethods DesktopClient { get; }
}