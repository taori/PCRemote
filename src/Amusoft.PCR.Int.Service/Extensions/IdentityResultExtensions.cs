using Microsoft.AspNetCore.Identity;

namespace Amusoft.PCR.Int.Service.Extensions;

public static class IdentityResultExtensions
{
	public static string ToLogMessage(this IdentityResult source)
	{
		return string.Join("; ", source.Errors.Select(d => $"{d.Code}, {d.Description}"));
	}
}