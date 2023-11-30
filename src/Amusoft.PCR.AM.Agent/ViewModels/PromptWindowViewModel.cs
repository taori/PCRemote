using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Amusoft.PCR.AM.Agent.ViewModels;

public partial class PromptWindowModel : ObservableValidator, IRecipient<GetPromptTextRequest>
{
	public PromptWindowModel()
	{
		WeakReferenceMessenger.Default.RegisterAll(this);
		ValidateAllProperties();
	}

	[ObservableProperty]
	private bool _isOpen;

	[ObservableProperty]
	private string? _title;

	[ObservableProperty]
	[Required]
	[MinLength(3)]
	[NotifyCanExecuteChangedFor(nameof(ConfirmAsyncCommand))]
	[NotifyDataErrorInfo]
	private string? _value = string.Empty;

	[ObservableProperty]
	private string? _description;

	[ObservableProperty]
	private string? _watermarkValue;

	private readonly TaskCompletionSource<GetPromptTextResponse> _completion = new();

	private bool CanConfirm() => !GetErrors(nameof(Value)).Any();

	[RelayCommand(CanExecute = nameof(CanConfirm))]
	public void ConfirmAsync()
	{
		WeakReferenceMessenger.Default.UnregisterAll(this);
		_completion.TrySetResult(new GetPromptTextResponse()
		{
			Cancelled = false,
			Content = Value
		});

		IsOpen = false;
	}

	[RelayCommand]
	public void CancelAsync()
	{
		WeakReferenceMessenger.Default.UnregisterAll(this);
		_completion.TrySetResult(new GetPromptTextResponse()
		{
			Cancelled = true,
			Content = string.Empty
		});

		IsOpen = false;
	}

	public void Receive(GetPromptTextRequest message)
	{
		Title = message.Title;
		Description = message.Description;
		WatermarkValue = message.WatermarkValue;

		message.Reply(_completion.Task);
	}
}

public class GetPromptTextResponse
{
	public bool Cancelled { get; init; }
	public string? Content { get; init; }
}

public class GetPromptTextRequest : AsyncRequestMessage<GetPromptTextResponse>
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