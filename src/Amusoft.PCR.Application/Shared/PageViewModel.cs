using Amusoft.PCR.Application.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NLog;

namespace Amusoft.PCR.Application.Shared;

public abstract partial class PageViewModel : ObservableObject, IDisposable
{
	private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
	
	protected readonly ITypedNavigator Navigator;

	[ObservableProperty]
	private string? _title;

	protected PageViewModel(ITypedNavigator navigator)
	{
		Navigator = navigator;
		Title = GetDefaultPageTitleImpl();
	}

	private string GetDefaultPageTitleImpl() => GetDefaultPageTitle();

	protected abstract string GetDefaultPageTitle();

	[RelayCommand(AllowConcurrentExecutions = false)]
	public Task GoBack()
	{
		return Navigator.PopAsync();
	}

	public void Dispose()
	{
		OnDispose(true);
		GC.SuppressFinalize(this);
	}


	private bool _disposed;
	private void Dispose(bool disposeManaged)
	{
		if (_disposed)
			return;

		try
		{
			OnDispose(disposeManaged);
		}
		catch (Exception e)
		{
			Log.Error(e, "Error occured while disposing viewmodel");
		}
		finally
		{
			_disposed = true;
		}
	}

	public virtual void OnDispose(bool disposeManaged){}
}