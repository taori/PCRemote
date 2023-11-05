using Amusoft.PCR.Application.Features.DesktopIntegration;
using Amusoft.PCR.Application.Services;

namespace Amusoft.PCR.Application.Extensions;

public static class DesktopIntegrationServiceExtensions
{
	public static TResult? Desktop<TResult>(this IDesktopIntegrationService? source, Func<IDesktopClientMethods, TResult> action)
	{
		if (source is { DesktopClient: { } client })
			return action(client);
		return default;
	}

}