namespace Amusoft.PCR.AM.UI.Interfaces;

public interface ICredentialUserPrompt
{
	Task<(string email, string password)?> SignInAsync();
}