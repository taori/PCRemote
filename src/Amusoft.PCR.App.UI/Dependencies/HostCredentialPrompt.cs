using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;

namespace Amusoft.PCR.App.UI.Dependencies;

public class HostCredentialPrompt : ICredentialPrompt
{
	private readonly IUserInterfaceService _userInterfaceService;

	public HostCredentialPrompt(IUserInterfaceService userInterfaceService)
	{
		_userInterfaceService = userInterfaceService;
	}

	public Task<string?> GetPasswordAsync(string subject)
	{
		return _userInterfaceService.GetPromptTextAsync(
			Translations.Generic_ActionRequired, string.Format(Translations.Generic_PleaseProvidePasswordFor_0, subject)
		);
	}
}