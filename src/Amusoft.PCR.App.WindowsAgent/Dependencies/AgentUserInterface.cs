using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Windows.Threading;
using Amusoft.PCR.AM.Agent.Interfaces;
using Amusoft.PCR.AM.Agent.ViewModels;
using Amusoft.PCR.App.WindowsAgent.Windows;
using Amusoft.PCR.Domain.Shared.Entities;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.App.WindowsAgent.Dependencies;

public class AgentUserInterface : IAgentUserInterface
{
	private readonly IServiceProvider _serviceProvider;

	public AgentUserInterface(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}
	
	public async Task<bool> ConfirmAsync(string title, string description)
	{
		var request = new GetConfirmRequest(title, description);
		var response = await GetWindowResponseAsync<ConfirmWindow, ConfirmWindowViewModel, GetConfirmRequest, GetConfirmResponse>(request);
		return response.Success;
	}

	public async Task<Result<string>> PromptPasswordAsync(string title, string description, string watermark)
	{
		var request = new GetPromptTextRequest(title, description, watermark);
		var response = await GetWindowResponseAsync<PromptWindow, PromptWindowModel, GetPromptTextRequest, GetPromptTextResponse>(request);
		return response.Cancelled ? Result.Error<string>() : Result.Success(response.Content ?? string.Empty);
	}
	
	
	public Task<TResponse> GetWindowResponseAsync<TWindow, TModel, TRequest, TResponse>(TRequest request)
		where TWindow : Window, new()
		where TModel : IRecipient<TRequest>, new()
		where TRequest : AsyncRequestMessage<TResponse>
		where TResponse : class
	{
		var tcs = new TaskCompletionSource<TResponse>();
		Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
		{
			if (Application.Current.MainWindow == null)
				return;

			var window = _serviceProvider.GetRequiredService<TWindow>();
			var model = _serviceProvider.GetRequiredService<TModel>();
			window.DataContext = model;

			window.Show();
			WeakReferenceMessenger.Default.Send(request);
			
			request.Response.ToObservable()
				.ObserveOn(SynchronizationContext.Current!)
				.Subscribe(d =>
				{
					tcs.TrySetResult(d);
					window.Close();
				});
		}));

		return tcs.Task;
	}
}