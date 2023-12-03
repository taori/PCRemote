using Amusoft.PCR.AM.Agent.Interfaces;
using Amusoft.PCR.Int.IPC;
using GrpcDotNetNamedPipes;
using NLog;

namespace Amusoft.PCR.Int.Agent.Dependencies;

public class InterprocessCommunicationServer : IInterprocessCommunicationServer
{
	private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
	
	private readonly DesktopIpcAgentLayer _ipcLayerDesktop;
	private readonly VoiceRecognitionIpcAgentLayer _ipcLayerVoice;
	private NamedPipeServer? _namedPipeServer;

	public InterprocessCommunicationServer(DesktopIpcAgentLayer ipcLayerDesktop, VoiceRecognitionIpcAgentLayer ipcLayerVoice)
	{
		_ipcLayerDesktop = ipcLayerDesktop;
		_ipcLayerVoice = ipcLayerVoice;
	}

	private bool TryLaunchInteropChannel()
	{
		try
		{
			Log.Debug("Initializing NamedPipeServer to listen for service calls");

			_namedPipeServer = new NamedPipeServer(Globals.NamedPipeChannel);
			DesktopIntegrationService.BindService(_namedPipeServer.ServiceBinder, _ipcLayerDesktop);
			VoiceCommandService.BindService(_namedPipeServer.ServiceBinder, _ipcLayerVoice);

			Log.Debug("Starting IPC Server");
			_namedPipeServer.Start();

			Log.Info("IPC Server is running");
			return true;
		}
		catch (Exception ex)
		{
			Log.Error(ex);
			return false;
		}
	}

	public void Start()
	{
		if (!TryLaunchInteropChannel())
		{
			Log.Fatal("Failed to launch named pipe for IPC with web application");
			_namedPipeServer?.Dispose();
		}
	}

	public void Stop()
	{
		Log.Debug("Shutting down named pipe server");
		_namedPipeServer?.Kill();
		_namedPipeServer?.Dispose();
	}
}