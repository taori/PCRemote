using System.Net;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IHostCredentialProvider
{
	IPEndPoint Address { get; }
	string Title { get; }

	/// <summary>
	/// http/https
	/// </summary>
	string Protocol { get; }
}