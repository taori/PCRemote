using Amusoft.PCR.AM.Agent;
using Amusoft.PCR.Int.WindowsAgent;
using NLog;

namespace Amusoft.PCR.App.WindowsAgent;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	private static readonly Logger Log = LogManager.GetCurrentClassLogger();

	private ApplicationModel? _applicationModel;

	protected override void OnStartup(StartupEventArgs e)
	{
		Log.Info("Launching Windows desktop integration");
		base.OnStartup(e);

		_applicationModel = new ApplicationModel();
		_applicationModel.OnStartup(services =>
		{
			services.AddWindowsAgentCore();
			services.AddWindowsAgentIntegration();
			services.AddWindowsAgentApplicationModel();
		});
	}

	protected override void OnExit(ExitEventArgs e)
	{
		Log.Info("Windows desktop integration shutting down");
		_applicationModel?.OnExit();
		
		base.OnExit(e);
	}
}