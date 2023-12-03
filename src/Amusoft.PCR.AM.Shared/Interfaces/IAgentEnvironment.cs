namespace Amusoft.PCR.AM.Shared.Interfaces;

public interface IAgentEnvironment
{
	string AgentName { get; }

	Task UpdateClipboardAsync(string? content);

	Task<string?> GetClipboardAsync();
}