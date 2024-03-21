namespace Amusoft.PCR.Domain.Service.Entities;

public record ServerConnection(
	int Port,
	string Protocol
);