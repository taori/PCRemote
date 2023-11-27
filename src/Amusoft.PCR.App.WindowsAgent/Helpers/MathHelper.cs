namespace Amusoft.PCR.App.WindowsAgent.Helpers;


public static class MathHelper
{
	public static bool IsEqual(float a, float b, float tolerance = 0.001f)
	{
		return Math.Abs(a - b) < tolerance;
	}
}