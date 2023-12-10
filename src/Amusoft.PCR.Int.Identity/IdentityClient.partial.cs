using System.Text;

namespace Amusoft.PCR.Int.Identity;

public partial class IdentityClient
{
	private Task PrepareRequestAsync(HttpClient client, HttpRequestMessage request, StringBuilder urlBuilder, CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	private Task PrepareRequestAsync(HttpClient client, HttpRequestMessage request, string url, CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	private Task ProcessResponseAsync(HttpClient client, HttpResponseMessage response, CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}