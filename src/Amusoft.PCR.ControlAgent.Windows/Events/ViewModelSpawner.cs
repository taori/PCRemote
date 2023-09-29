using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.Mvvm.Messaging;
using System.Threading.Tasks;
using System.Windows.Threading;
using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Windows;

namespace Amusoft.PCR.ControlAgent.Windows.Events;


public static class ViewModelSpawner
{
	public static Task<TResponse> GetWindowResponseAsync<TWindow, TModel, TRequest, TResponse>(TRequest request)
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

			var window = new TWindow();
			var model = new TModel();
			window.DataContext = model;

			window.Show();
			WeakReferenceMessenger.Default.Send(request);

			request.Response.ToObservable()
				.ObserveOnDispatcher()
				.Subscribe(d =>
				{
					tcs.TrySetResult(d);
					window.Close();
				});
		}));

		return tcs.Task;
	}
}