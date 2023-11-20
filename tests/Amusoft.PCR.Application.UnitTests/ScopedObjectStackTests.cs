using Amusoft.PCR.Application.Utility;
using Shouldly;

namespace Amusoft.PCR.Application.UnitTests;

public class ScopedObjectStackTests
{
	[Fact]
	public void DefaultIsEmpty()
	{
		using var stack = new NavigationScope();
		stack.GetValues().Count().ShouldBe(0);
	}

	[Fact]
	public void Addition()
	{
		using var stack = new NavigationScope();
		stack.GetValues().Count().ShouldBe(0);
		stack.Push(new object());
		stack.GetValues().Count().ShouldBe(1);
	}

	[Fact]
	public void QueryScope()
	{
		var valueA = new object();

		using (var outer = new NavigationScope())
		{
			outer.Push(valueA);
			outer.GetValues().Count().ShouldBe(1);

			NavigationScope.TryGetScope(out var scope).ShouldBeTrue();
			scope.ShouldNotBeNull();
			scope.GetValues().Count.ShouldBe(1);
		}

		NavigationScope.TryGetScope(out var scope2).ShouldBeFalse();
		scope2.ShouldBeNull();
	}
}