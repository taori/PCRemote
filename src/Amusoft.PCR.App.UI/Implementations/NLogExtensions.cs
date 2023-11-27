using Amusoft.PCR.App.UI.Platforms;
using NLog;
using NLog.Config;

namespace Amusoft.PCR.App.UI.Implementations;

/// <summary>
/// Extension methods to setup NLog <see cref="LoggingConfiguration"/>
/// </summary>
public static class SetupLoadConfigurationExtensions
{
	/// <summary>
	/// Write to MAUI NLog Target
	/// </summary>
	/// <param name="configBuilder"></param>
	/// <param name="layout">Override the default Layout for output</param>
	/// <param name="category">Override the logging category</param>
	public static ISetupConfigurationTargetBuilder WriteToMauiLogCustom(this ISetupConfigurationTargetBuilder configBuilder, NLog.Layouts.Layout layout = null, NLog.Layouts.Layout category = null)
	{
		var logTarget = new MauiLog();
		
		if (layout != null)
			logTarget.Layout = layout;
		if (category != null)
			logTarget.Category = category;
		return configBuilder.WriteTo(logTarget);
	}
}