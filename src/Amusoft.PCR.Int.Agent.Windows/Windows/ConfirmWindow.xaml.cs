using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Amusoft.PCR.Int.Agent.Windows.Events;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Amusoft.PCR.Int.Agent.Windows.Windows;

/// <summary>
/// Interaction logic for ConfirmWindow.xaml
/// </summary>
public partial class ConfirmWindow
{
	public ConfirmWindow()
	{
		InitializeComponent();
	}
}

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