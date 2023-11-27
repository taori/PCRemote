using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Amusoft.PCR.App.WindowsAgent.Events;


public static class ViewModelSpawner
{
	public static Task<TResponse> GetWindowResponseAsync<TWindow, TModel, TRequest, TResponse>(TRequest request)
		where TWindow : Window, new()
		where TModel : IRecipient<TRequest>, new()
		where TRequest : AsyncRequestMessage<TResponse>
		where TResponse : class
	{
		var tcs = new TaskCompletionSource<TResponse>();
		System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
		{
			if (System.Windows.Application.Current.MainWindow == null)
				return;

			var window = new TWindow();
			var model = new TModel();
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