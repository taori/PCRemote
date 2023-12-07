namespace Amusoft.PCR.Domain.Service.ValueTypes;

public enum VoiceRecognitionPhaseKind
{
	VoiceRecognitionEnabled = 0,
	VoiceRecognitionTriggerWord = 1,
	VoiceRecognitionTriggerWordAudio = 2,
	VoiceRecognitionTriggerWordOnAliases = 3,
	VoiceRecognitionTriggerWordOffAliases = 4,
	VoiceRecognitionConfirmMessage = 5,
	VoiceRecognitionErrorMessage = 6,
	VoiceRecognitionThreshold = 7,
	VoiceRecognitionTriggerWordAudioMaster = 8,
}