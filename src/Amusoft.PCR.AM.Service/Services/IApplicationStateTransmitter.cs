namespace Amusoft.PCR.Domain.Services;

public interface IApplicationStateTransmitter
{
	Task ConfigurationDone { get; }
	void NotifyConfigurationDone();
}