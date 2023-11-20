using Amusoft.PCR.Application.Features.DesktopIntegration;
using Amusoft.PCR.Application.Services;

namespace Amusoft.PCR.Application.Extensions;

public static class DesktopIntegrationServiceExtensions
{
	public static Task<TResult> Desktop<TResult>(this IDesktopIntegrationService? source, Func<IDesktopClientMethods, Task<TResult>> action)
		where TResult : new()
	{
		if (source is { DesktopClient: { } client })
			return action(client);
		return Task.FromResult(new TResult());
	}

	public static Task<string?> Desktop(this IDesktopIntegrationService source, Func<IDesktopClientMethods, Task<string?>> action)
	{
		if (source is { DesktopClient: { } client })
			return action(client);
		return Task.FromResult(default(string?));
	}

	public static Task Desktop(this IDesktopIntegrationService? source, Func<IDesktopClientMethods, Task> action)
	{
		if (source is { DesktopClient: { } client })
			return action(client);

		return Task.CompletedTask;
	}
}