using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Amusoft.PCR.Domain.UI.Entities;

public class BearerToken
{
	public BearerToken()
	{
	}

	[SetsRequiredMembers]
	public BearerToken(string address, string accessToken, string refreshToken, DateTimeOffset expires)
	{
		Address = address;
		AccessToken = accessToken;
		RefreshToken = refreshToken;
		Expires = expires;
	}

	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public Guid Id { get; set; }

	public required string Address { get; set; }

	public required string AccessToken { get; set; }

	public required string RefreshToken { get; set; }

	public required DateTimeOffset Expires { get; set; }
}