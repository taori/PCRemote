namespace Amusoft.PCR.Application.Services;

public interface IAgentEnvironment
{
	string AgentName { get; }

	Task UpdateClipboardAsync(string? content);

	Task<string?> GetClipboardAsync();
}