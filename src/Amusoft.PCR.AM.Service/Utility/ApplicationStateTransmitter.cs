using Amusoft.PCR.AM.Service.Interfaces;

namespace Amusoft.PCR.AM.Service.Utility;

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