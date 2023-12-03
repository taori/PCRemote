namespace Amusoft.PCR.AM.Shared.Utility;

public class Cooldown
{
	private readonly long _cooldownLength;
	private readonly object _lock = new();

	public Cooldown(TimeSpan cooldownLength)
	{
		_cooldownLength = cooldownLength.Ticks;
	}
	
	private long _lastClaim = long.MinValue;
	public bool TryClaim()
	{
		if (Monitor.TryEnter(_lock))
		{
			try
			{
				var nowTicks = DateTime.Now.Ticks;
				if (_lastClaim <= nowTicks)
				{
					_lastClaim = nowTicks + _cooldownLength;
					return true;
				}
				else
				{
					return false;
				}
			}
			finally
			{
				Monitor.Exit(_lock);
			}
		}
		else
		{
			return false;
		}
	}
}