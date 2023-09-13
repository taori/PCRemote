using System;
using System.Collections.Generic;
using System.Linq;
using WixToolset.Dtf.WindowsInstaller;

namespace Amusoft.PCR.Installer.Custom
{
	public class CustomActions
	{
		[CustomAction]
		public static ActionResult CustomAction1(Session session)
		{
			session.Log("Begin CustomAction1");

			return ActionResult.Success;
		}
	}
}
