using System;
using System.Threading.Tasks;
using Amusoft.PCR.Int.IPC;
using Grpc.Core;
using NLog;

namespace Amusoft.PCR.Int.Agent.Windows.Services;

public class VoiceRecognitionServiceImplementation : VoiceCommandService.VoiceCommandServiceBase
{
	private static readonly Logger Log = LogManager.GetCurrentClassLogger();

	public override Task<DefaultResponse> UpdateVoiceRecognition(UpdateVoiceRecognitionRequest request, ServerCallContext context)
	{
		try
		{
			// SpeechManager.Instance.UpdateGrammar(request);
			return Task.FromResult(new DefaultResponse() { Success = true });
		}
		catch (Exception e)
		{
			Log.Error(e, nameof(UpdateVoiceRecognition));

			throw new RpcException(Status.DefaultCancelled, "Failed to update voice recognition");
		}
	}

	public override Task<DefaultResponse> StartVoiceRecognition(DefaultRequest request, ServerCallContext context)
	{
		try
		{
			// SpeechManager.Instance.StartVoiceRecognition();
			return Task.FromResult(new DefaultResponse() { Success = true });
		}
		catch (Exception e)
		{
			Log.Error(e, nameof(StartVoiceRecognition));

			throw new RpcException(Status.DefaultCancelled, "Failed to start voice recognition");
		}
	}

	public override Task<DefaultResponse> StopVoiceRecognition(DefaultRequest request, ServerCallContext context)
	{
		try
		{
			// SpeechManager.Instance.StopVoiceRecognition();
			return Task.FromResult(new DefaultResponse() { Success = true });
		}
		catch (Exception e)
		{
			Log.Error(e, nameof(StopVoiceRecognition));

			throw new RpcException(Status.DefaultCancelled, "Failed to stop voice recognition");
		}
	}
}