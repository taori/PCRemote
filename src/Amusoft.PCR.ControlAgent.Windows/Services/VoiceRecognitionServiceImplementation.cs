using Amusoft.PCR.ControlAgent.Shared;
using Amusoft.PCR.Grpc.Common;
using Grpc.Core;
using NLog;
using System.Threading.Tasks;
using System;

namespace Amusoft.PCR.ControlAgent.Windows.Services;

public class VoiceRecognitionServiceImplementation : VoiceCommandService.VoiceCommandServiceBase
{
	private static readonly Logger Log = LogManager.GetLogger(nameof(VoiceRecognitionServiceImplementation));

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