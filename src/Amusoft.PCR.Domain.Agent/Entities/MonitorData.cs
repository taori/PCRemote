namespace Amusoft.PCR.Domain.Agent.Entities;

public record MonitorData(string Id, string Description, uint Current, uint Min, uint Max);