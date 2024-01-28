namespace Amusoft.PCR.App.Service.Environment;

public static class WildcardReplacements
{
	public static string ReplaceAppDir(string input) => input.Replace("{AppDir}", AppContext.BaseDirectory);
}