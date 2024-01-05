using Amusoft.PCR.AM.UI.ViewModels;
using Amusoft.PCR.Domain.Shared.ValueTypes;
using Shouldly;

namespace Amusoft.PCR.AM.UI.UnitTests;

public class HostAccountPermissionsViewModelTests
{
	[Fact]
	public void RoleIdMatch()
	{
		var roleA = new HostAccountPermissionCheckboxModel.RoleId("a");
		var roleB = new HostAccountPermissionCheckboxModel.RoleId("b");
		var roleC = new HostAccountPermissionCheckboxModel.RoleId("c");

		roleA.ShouldBe(roleA);
		roleA.ShouldNotBe(roleB);
		roleA.ShouldNotBe(roleC);

		roleB.ShouldBe(roleB);
		roleB.ShouldNotBe(roleA);
		roleB.ShouldNotBe(roleC);

		roleC.ShouldBe(roleC);
		roleC.ShouldNotBe(roleA);
		roleC.ShouldNotBe(roleB);
	}

	[Fact]
	public void UserPermissionIdMatch()
	{
		var baseLine = new HostAccountPermissionCheckboxModel.UserPermissionId((PermissionKind.HostCommand, "1", "1"));
		baseLine.Equals(new HostAccountPermissionCheckboxModel.UserPermissionId((PermissionKind.HostCommand, "1", "1"))).ShouldBeTrue();
		baseLine.Equals(new HostAccountPermissionCheckboxModel.UserPermissionId((PermissionKind.HostCommand, "1", "2"))).ShouldBeFalse();
		baseLine.Equals(new HostAccountPermissionCheckboxModel.UserPermissionId((PermissionKind.HostCommand, "2", "1"))).ShouldBeFalse();
		baseLine.Equals(new HostAccountPermissionCheckboxModel.UserPermissionId((PermissionKind.HostCommand, "2", "2"))).ShouldBeFalse();
	}

	[Fact]
	public void CompareCheckBoxStates()
	{
		var baseLine = new HostAccountPermissionCheckboxModel(
			new HostAccountPermissionCheckboxModel.UserPermissionId((PermissionKind.HostCommand, "1", "1")),
			HostAccountPermissionCheckboxModel.Category.Roles
		)
		{
			Checked = true
		};
		baseLine.Equals(
			new HostAccountPermissionCheckboxModel(new HostAccountPermissionCheckboxModel.UserPermissionId((PermissionKind.HostCommand, "1", "1")), HostAccountPermissionCheckboxModel.Category.Roles)
			{
				Checked = true
			}
		).ShouldBeTrue();
		baseLine.Equals(
			new HostAccountPermissionCheckboxModel(new HostAccountPermissionCheckboxModel.UserPermissionId((PermissionKind.HostCommand, "1", "1")), HostAccountPermissionCheckboxModel.Category.Roles)
			{
				Checked = false
			}
		).ShouldBeFalse();
	}
}