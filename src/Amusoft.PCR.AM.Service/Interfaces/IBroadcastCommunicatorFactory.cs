using Amusoft.PCR.Domain.Service.Entities;

namespace Amusoft.PCR.AM.Service.Interfaces;

public interface IBroadcastCommunicatorFactory
{
	IBroadcastCommunicator Create(ServerUrlTransmitterSettings settings);
}