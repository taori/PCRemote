using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Amusoft.PCR.Domain.Service.Entities;

public class HostCommand
{
	[MaxLength(40)]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public string Id { get; set; }

	[MaxLength(128)]
	public string CommandName { get; set; }

	[MaxLength(512)]
	public string ProgramPath { get; set; }

	[MaxLength(1024)]
	public string? Arguments { get; set; }
}