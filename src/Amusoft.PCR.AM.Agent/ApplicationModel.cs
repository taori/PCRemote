using Amusoft.PCR.AM.Agent.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.PCR.AM.Agent;

public class ApplicationModel : IDisposable, IAsyncDisposable
{
	private IApplicationHost? _applicationEventHandler;
	private ServiceProvider? _serviceProvider;

	public void OnStartup(Action<IServiceCollection> configureServices)
	{
		var serviceCollection = new ServiceCollection();
		configureServices(serviceCollection);

		_serviceProvider = serviceCollection.BuildServiceProvider();
		_applicationEventHandler = _serviceProvider.GetRequiredService<IApplicationHost>();
		_applicationEventHandler.ExecuteStartup();
	}

	public void OnExit()
	{
		_applicationEventHandler?.ExecuteShutdown();
	}

	public void Dispose()
	{
		_serviceProvider?.Dispose();
	}

	public async ValueTask DisposeAsync()
	{
		if (_serviceProvider != null) 
			await _serviceProvider.DisposeAsync();
	}
}