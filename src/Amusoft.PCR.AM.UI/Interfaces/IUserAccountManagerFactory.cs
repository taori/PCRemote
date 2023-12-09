#region

using System.Net;

#endregion

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IUserAccountManagerFactory
{
	IUserAccountManager Create(IPEndPoint endPoint);
}