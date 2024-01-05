using System.Runtime.CompilerServices;
using DiffEngine;

namespace Amusoft.PCR.AM.UI.UnitTests;

public class VerifyTestConfiguration
{
	[ModuleInitializer]
	public static void Initialize()
	{
		DiffTools.UseOrder(DiffTool.Rider, DiffTool.VisualStudioCode, DiffTool.VisualStudio);

		VerifierSettings.UseSplitModeForUniqueDirectory();

		DerivePathInfo(
			(sourceFile, projectDirectory, type, method) => new PathInfo(
				directory: Path.Combine(projectDirectory, "Snapshots"),
				typeName: type.Name,
				methodName: method.Name));
	}
}