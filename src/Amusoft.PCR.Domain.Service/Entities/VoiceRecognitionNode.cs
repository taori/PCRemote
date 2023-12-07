using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Amusoft.PCR.Domain.Service.Entities;

public class VoiceRecognitionNode
{
	[MaxLength(45)]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public string Id { get; set; }

	public string Name { get; set; }

	public ICollection<VoiceRecognitionNodeAlias> Aliases { get; set; }
}