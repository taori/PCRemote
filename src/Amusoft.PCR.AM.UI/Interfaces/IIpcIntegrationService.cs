using Amusoft.PCR.Domain.Shared.Interfaces;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IIpcIntegrationService
{
	IDesktopClientMethods DesktopClient { get; }

	IIdentityExtendedClient IdentityExtendedClient { get; }
}