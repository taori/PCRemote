﻿using System.Net;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IDesktopIntegrationServiceFactory
{
	IIpcIntegrationService Create(string protocol, IPEndPoint endPoint);
}