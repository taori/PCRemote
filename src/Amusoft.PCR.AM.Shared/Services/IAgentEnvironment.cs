namespace Amusoft.PCR.AM.Shared.Services;

public interface IAgentEnvironment
{
	string AgentName { get; }

	Task UpdateClipboardAsync(string? content);

	Task<string?> GetClipboardAsync();
}