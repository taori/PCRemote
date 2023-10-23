namespace Amusoft.PCR.Domain.Services;

public interface IWlanBroadcasterFactory
{
	IWlanBroadcaster Create(int port);
}

public interface IWlanBroadcaster
{
	Task SendAsync(byte[] content, CancellationToken cancellationToken);
}