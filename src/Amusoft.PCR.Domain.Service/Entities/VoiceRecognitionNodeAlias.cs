using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Amusoft.PCR.Domain.Service.Entities;

public class VoiceRecognitionNodeAlias
{
	[MaxLength(45)]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public string Id { get; set; }

	public string FeedId { get; set; }

	[ForeignKey(nameof(FeedId))]
	public VoiceRecognitionNode Feed { get; set; }

	public string Alias { get; set; }
}