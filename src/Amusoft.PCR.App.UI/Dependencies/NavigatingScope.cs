using Amusoft.PCR.AM.UI.Interfaces;

namespace Amusoft.PCR.App.UI.Dependencies;

public class NavigatingScope : INavigatingContext
{
	private readonly ShellNavigatingEventArgs _args;

	public NavigatingScope(ShellNavigatingEventArgs args)
	{
		_args = args;
	}

	public IDeferalScope PauseNavigation()
	{
		return new DeferalScope(_args);
	}

	private class DeferalScope : IDeferalScope
	{
		private ShellNavigatingEventArgs? _args;
		private ShellNavigatingDeferral? _deferal;

		public DeferalScope(ShellNavigatingEventArgs args)
		{
			_args = args;
			_deferal = args.GetDeferral();
		}

		public ValueTask DisposeAsync()
		{
			_deferal?.Complete();
			_deferal = null;
			_args = null;
			return ValueTask.CompletedTask;
		}

		public void Cancel()
		{
			if (_args is not null)
			{
				if (!_args.Cancelled && _args.CanCancel)
					_args.Cancel();
			}
		}
	}
}