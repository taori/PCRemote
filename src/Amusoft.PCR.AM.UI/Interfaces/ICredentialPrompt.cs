namespace Amusoft.PCR.AM.UI.Interfaces;

public interface ICredentialPrompt
{
	Task<string?> GetPasswordAsync(string subject);
}