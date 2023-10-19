using Amusoft.PCR.Domain.Services;

namespace Amusoft.PCR.Application.Utility;

public class ApplicationStateTransmitter : IApplicationStateTransmitter
{
	private readonly TaskCompletionSource _configurationDone;

	public ApplicationStateTransmitter()
	{
		_configurationDone = new TaskCompletionSource(null);
	}

	public Task ConfigurationDone => _configurationDone.Task;

	public void NotifyConfigurationDone() => _configurationDone.SetResult();
}