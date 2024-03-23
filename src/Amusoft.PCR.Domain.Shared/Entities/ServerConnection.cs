namespace Amusoft.PCR.Domain.Shared.Entities;

public record ServerConnection(
	int Port,
	string Protocol
);