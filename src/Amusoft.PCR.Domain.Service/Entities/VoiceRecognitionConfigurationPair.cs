using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Amusoft.PCR.Domain.Service.ValueTypes;

namespace Amusoft.PCR.Domain.Service.Entities;

[DebuggerDisplay("{Key} => {Value}")]
public class VoiceRecognitionConfigurationPair
{
	[Key]
	public VoiceRecognitionPhaseKind Key { get; set; }

	public string Value { get; set; }
}