using Amusoft.PCR.Application.Services;

namespace Amusoft.PCR.App.UI.Implementations;

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