﻿using Amusoft.PCR.ControlAgent.Windows.Events;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls;

namespace Amusoft.PCR.ControlAgent.Windows.Windows
{
	/// <summary>
	/// Interaction logic for PromptWindow.xaml
	/// </summary>
	public partial class PromptWindow : MetroWindow
	{
		public PromptWindow()
		{
			InitializeComponent();
		}
	}

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
		private string _title;

		[ObservableProperty]
		[Required]
		[MinLength(3)]
		[NotifyCanExecuteChangedFor(nameof(ConfirmAsyncCommand))]
		[NotifyDataErrorInfo]
		private string _value = string.Empty;

		[ObservableProperty]
		private string _description;

		[ObservableProperty]
		private string _watermarkValue;

		private readonly TaskCompletionSource<PromptCompleted> _completion = new();

		private bool CanConfirm() => !GetErrors(nameof(Value)).Any();

		[RelayCommand(CanExecute = nameof(CanConfirm))]
		public void ConfirmAsync()
		{
			WeakReferenceMessenger.Default.UnregisterAll(this);
			_completion.TrySetResult(new PromptCompleted()
			{
				Cancelled = false,
				Content = _value
			});

			IsOpen = false;
		}

		[RelayCommand]
		public void CancelAsync()
		{
			WeakReferenceMessenger.Default.UnregisterAll(this);
			_completion.TrySetResult(new PromptCompleted()
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

	public class PromptCompleted
	{
		public bool Cancelled { get; init; }
		public string? Content { get; init; }
	}
}