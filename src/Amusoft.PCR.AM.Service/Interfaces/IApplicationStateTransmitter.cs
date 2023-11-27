namespace Amusoft.PCR.AM.Service.Interfaces;

public interface IApplicationStateTransmitter
{
	Task ConfigurationDone { get; }
	void NotifyConfigurationDone();
}