using System.Text;

namespace Amusoft.PCR.Int.Identity;

public partial class IdentityClient
{
	public List<PrepareModification> PrepareModifications = new();
	
	private Task PrepareRequestAsync(HttpClient client, HttpRequestMessage request, StringBuilder urlBuilder, CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	private Task PrepareRequestAsync(HttpClient client, HttpRequestMessage request, string url, CancellationToken cancellationToken)
	{
		var modifications = PrepareModifications.Where(d => d.IsUrlMatch(url));
		foreach (var modification in modifications)
		{
			modification.Modification(request);
		}
		return Task.CompletedTask;
	}

	private Task ProcessResponseAsync(HttpClient client, HttpResponseMessage response, CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}

public class PrepareModification
{
	public PrepareModification(Predicate<string> isUrlMatch, Action<HttpRequestMessage> modification)
	{
		IsUrlMatch = isUrlMatch;
		Modification = modification;
	}

	public Predicate<string> IsUrlMatch { get; set; }

	public Action<HttpRequestMessage> Modification { get; set; }
}