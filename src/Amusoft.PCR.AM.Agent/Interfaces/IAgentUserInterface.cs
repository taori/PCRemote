using Amusoft.PCR.Domain.Shared.Entities;

namespace Amusoft.PCR.AM.Agent.Interfaces;

public interface IAgentUserInterface
{
	Task<bool> ConfirmAsync(string title, string description);
	Task<Result<string>> PromptPasswordAsync(string title, string description, string watermark);
}