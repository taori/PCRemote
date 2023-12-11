using System.Net;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IUserAccountManagerFactory
{
	IUserAccountManager Create(IPEndPoint endPoint, string protocol);
}