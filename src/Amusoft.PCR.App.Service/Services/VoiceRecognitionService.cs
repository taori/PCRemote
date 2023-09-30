using Amusoft.PCR.Int.IPC;
using Grpc.Core;

namespace Amusoft.PCR.App.Service.Services;

public class VoiceRecognitionService : VoiceCommandService.VoiceCommandServiceBase
{
	private readonly ILogger<VoiceRecognitionService> _logger;

	public VoiceRecognitionService(ILogger<VoiceRecognitionService> logger)
	{
		_logger = logger;
	}

	public override Task<DefaultResponse> UpdateVoiceRecognition(UpdateVoiceRecognitionRequest request, ServerCallContext context)
	{
		_logger.LogInformation("{Method} called", nameof(UpdateVoiceRecognition));
		return Task.FromResult(new DefaultResponse());
	}

	public override Task<DefaultResponse> StopVoiceRecognition(DefaultRequest request, ServerCallContext context)
	{
		_logger.LogInformation("{Method} called", nameof(StopVoiceRecognition));
		return Task.FromResult(new DefaultResponse());
	}

	public override Task<DefaultResponse> StartVoiceRecognition(DefaultRequest request, ServerCallContext context)
	{
		_logger.LogInformation("{Method} called", nameof(StartVoiceRecognition));
		return Task.FromResult(new DefaultResponse());
	}
}