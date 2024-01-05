using System.Diagnostics.CodeAnalysis;

namespace Amusoft.PCR.Domain.Shared.Entities;

public class UserRole
{
	[SetsRequiredMembers]
	public UserRole(string id, string name, bool granted)
	{
		Id = id;
		Name = name;
		Granted = granted;
	}

	public UserRole()
	{
	}

	public required string Id { get; set; }

	public required string Name { get; set; }

	public required bool Granted { get; set; }
}