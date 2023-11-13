﻿using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Amusoft.PCR.Int.Agent.Windows.Events;

public class GetConfirmRequest : AsyncRequestMessage<GetConfirmResponse>
{
	public string Title { get; set; }

	public string Description { get; set; }
}


public class GetConfirmResponse
{
	public GetConfirmResponse(bool success)
	{
		Success = success;
	}

	public bool Success { get; set; }
}