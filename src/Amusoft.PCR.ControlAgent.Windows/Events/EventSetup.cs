using Amusoft.PCR.ControlAgent.Windows.Windows;
using System.Diagnostics;

namespace Amusoft.PCR.ControlAgent.Windows.Events;

public static class EventSetup
{
	public static void Initialize()
	{
	}

	[Conditional("DEBUG")]
	public static async void Debug()
	{
		// todo refactoring. Knowing all related types would be bad design to get a reply for a request
		var request = new GetPromptTextRequest("Prompt", "Please enter a valid value", "Password");
		var response = await ViewModelSpawner.GetWindowResponseAsync<PromptWindow, PromptWindowModel, GetPromptTextRequest, PromptCompleted>(request);
	}
}