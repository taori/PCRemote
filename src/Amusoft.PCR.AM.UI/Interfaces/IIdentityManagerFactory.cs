using System.Net;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IIdentityManagerFactory
{
	IIdentityManager Create(IPEndPoint endPoint, string protocol);
}