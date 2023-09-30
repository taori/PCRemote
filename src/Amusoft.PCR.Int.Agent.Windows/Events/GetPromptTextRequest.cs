using Amusoft.PCR.Int.Agent.Windows.Windows;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Amusoft.PCR.Int.Agent.Windows.Events;

public class GetPromptTextRequest : AsyncRequestMessage<PromptCompleted>
{
	public GetPromptTextRequest(string title, string description, string watermarkValue)
	{
		Title = title;
		Description = description;
		WatermarkValue = watermarkValue;
	}

	public string Title { get; set; }

	public string Description { get; set; }

	public string WatermarkValue { get; set; }
}