using Amusoft.PCR.Int.IPC;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.Service.Services;

public class VoiceRecognitionCommandReceiver : VoiceCommandService.VoiceCommandServiceBase
{
	private readonly ILogger<VoiceRecognitionCommandReceiver> _logger;

	public VoiceRecognitionCommandReceiver(ILogger<VoiceRecognitionCommandReceiver> logger)
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