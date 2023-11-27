using Amusoft.PCR.AM.Shared.Utility;
using Shouldly;

namespace Amusoft.PCR.AM.UI.UnitTests;

public class CooldownTests
{
	[Theory]
	[InlineData(500)]
	[InlineData(1000)]
	public async Task TestLockTaken(long cd)
	{
		var cooldown = new Cooldown(TimeSpan.FromMilliseconds(cd));
		cooldown.TryClaim().ShouldBe(true);
		await Task.Delay(TimeSpan.FromMilliseconds(cd - 50));
		cooldown.TryClaim().ShouldBe(false);
		await Task.Delay(TimeSpan.FromMilliseconds(100));
		cooldown.TryClaim().ShouldBe(true);
	}
}