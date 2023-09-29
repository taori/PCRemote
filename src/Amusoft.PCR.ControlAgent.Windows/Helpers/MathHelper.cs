using System;

namespace Amusoft.PCR.ControlAgent.Windows.Helpers;


public static class MathHelper
{
	public static bool IsEqual(float a, float b, float tolerance = 0.001f)
	{
		return Math.Abs(a - b) < tolerance;
	}
}