using Amusoft.PCR.AM.Agent.Interfaces;
using Amusoft.PCR.Int.IPC;
using Grpc.Core;

namespace Amusoft.PCR.Int.Agent.Dependencies;

public class VoiceRecognitionIpcAgentLayer : VoiceCommandService.VoiceCommandServiceBase
{
	private readonly IVoiceRecognitionProcessor _processor;

	public VoiceRecognitionIpcAgentLayer(IVoiceRecognitionProcessor processor)
	{
		_processor = processor;
	}
	
	public override Task<DefaultResponse> UpdateVoiceRecognition(UpdateVoiceRecognitionRequest request, ServerCallContext context)
	{
		throw new NotImplementedException();
	}

	public override Task<DefaultResponse> StopVoiceRecognition(DefaultRequest request, ServerCallContext context)
	{
		throw new NotImplementedException();
	}

	public override Task<DefaultResponse> StartVoiceRecognition(DefaultRequest request, ServerCallContext context)
	{
		throw new NotImplementedException();
	}
}