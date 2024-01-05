using CommunityToolkit.Mvvm.ComponentModel;

namespace Amusoft.PCR.AM.Shared.Utility;

public partial class LoadState : ObservableObject
{
	[ObservableProperty] private bool _loading;

	private int _loadCounter;

	public IDisposable QueueLoading() => new Token(this);

	private void Decrement()
	{
		_loadCounter--;
		Loading = _loadCounter != 0;
	}

	private void Increment()
	{
		_loadCounter++;
		Loading = _loadCounter != 0;
	}

	private class Token : IDisposable
	{
		private readonly LoadState _loadState;

		public Token(LoadState loadState)
		{
			_loadState = loadState;
			_loadState.Increment();
		}

		public void Dispose()
		{
			_loadState.Decrement();
		}
	}
}