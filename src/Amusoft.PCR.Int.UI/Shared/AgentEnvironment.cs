using Amusoft.PCR.AM.Shared.Interfaces;

namespace Amusoft.PCR.Int.UI.Shared;

public class AgentEnvironment : IAgentEnvironment
{
	public string AgentName => DeviceInfo.Name;

	public Task UpdateClipboardAsync(string? content)
	{
		return Clipboard.Default.SetTextAsync(content);
	}

	public Task<string?> GetClipboardAsync()
	{
		return Clipboard.Default.GetTextAsync();
	}
}