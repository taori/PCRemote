namespace Amusoft.PCR.Domain.UI.Entities;

public record SignInResponse(string AccessToken, string RefreshToken, DateTimeOffset ValidUntil);