using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Amusoft.PCR.Domain.UI.Entities;

public class BearerToken
{
	public BearerToken()
	{
	}

	[SetsRequiredMembers]
	public BearerToken(string accessToken, string refreshToken, DateTimeOffset expires, DateTimeOffset issuedAt, Guid endpointAccountId)
	{
		AccessToken = accessToken;
		RefreshToken = refreshToken;
		Expires = expires;
		IssuedAt = issuedAt;
		EndpointAccountId = endpointAccountId;
	}

	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public Guid Id { get; set; }

	public required Guid EndpointAccountId { get; set; }

	public EndpointAccount? EndpointAccount { get; set; }

	public required DateTimeOffset IssuedAt { get; set; }

	public required string AccessToken { get; set; }

	public required string RefreshToken { get; set; }

	public required DateTimeOffset Expires { get; set; }
}