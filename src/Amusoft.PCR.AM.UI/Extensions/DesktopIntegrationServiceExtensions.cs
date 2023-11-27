using Amusoft.PCR.AM.Shared.Interfaces;
using Amusoft.PCR.AM.UI.Interfaces;

namespace Amusoft.PCR.AM.UI.Extensions;

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