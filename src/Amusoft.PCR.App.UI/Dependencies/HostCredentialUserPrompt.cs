#region

using Amusoft.PCR.AM.UI.Interfaces;

#endregion

namespace Amusoft.PCR.App.UI.Implementations;

public class HostCredentialUserPrompt : ICredentialUserPrompt
{
	private readonly IUserInterfaceService _userInterfaceService;

	public HostCredentialUserPrompt(IUserInterfaceService userInterfaceService)
	{
		_userInterfaceService = userInterfaceService;
	}

	public async Task<(string email, string password)?> SignInAsync()
	{
		// todo show customized dialog
		if (await _userInterfaceService.GetPromptTextAsync("Email", "Provide the email please") is { } email
		    && await _userInterfaceService.GetPromptTextAsync("Password", "Provide the password please") is { } password)
		{
			return (email, password);
		}

		return default;
	}
}