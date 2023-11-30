using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Amusoft.PCR.AM.Agent.ViewModels;

public partial class ConfirmWindowViewModel : ObservableObject, IRecipient<GetConfirmRequest>
{
	public ConfirmWindowViewModel()
	{
		WeakReferenceMessenger.Default.RegisterAll(this);
	}

	[ObservableProperty]
	private string? _title;

	[ObservableProperty]
	private string? _description;

	private TaskCompletionSource<GetConfirmResponse> _decision = new();

	public Task<GetConfirmResponse> DecisionMade => _decision.Task;

	[RelayCommand]
	private void Confirm()
	{
		WeakReferenceMessenger.Default.UnregisterAll(this);
		_decision.TrySetResult(new GetConfirmResponse(true));
	}

	[RelayCommand]
	private void Decline()
	{
		WeakReferenceMessenger.Default.UnregisterAll(this);
		_decision.TrySetResult(new GetConfirmResponse(false));
	}

	public void Receive(GetConfirmRequest message)
	{
		Title = message.Title;
		Description = message.Description;

		message.Reply(_decision.Task);
	}
}

public class GetConfirmRequest : AsyncRequestMessage<GetConfirmResponse>
{
	public GetConfirmRequest(string title, string description)
	{
		Title = title;
		Description = description;
	}

	public string Title { get; }

	public string Description { get; }
}


public class GetConfirmResponse
{
	public GetConfirmResponse(bool success)
	{
		Success = success;
	}

	public bool Success { get; }
}