using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class HostAccountPermissionsViewModel : ReloadablePageViewModel
{
	public HostAccountPermissionsViewModel(ITypedNavigator navigator) : base(navigator)
	{
	}

	[ObservableProperty]
	private List<CheckboxGroup> _items = new();

	protected override string GetDefaultPageTitle()
	{
		return Translations.HostAccountPermissions_Title;
	}

	protected override Task OnReloadAsync(CancellationToken cancellationToken)
	{
		_items.Add(new CheckboxGroup("Test1")
		{
			Items =
			{
				new HostAccountPermissionCheckboxModel() { Text = "test1" }
				, new HostAccountPermissionCheckboxModel() { Text = "test2", Checked = true }
				,
			}
		});
		_items.Add(new CheckboxGroup("Test2")
		{
			Items =
			{
				new HostAccountPermissionCheckboxModel() { Text = "test1" }
				, new HostAccountPermissionCheckboxModel() { Text = "test2", Checked = true }
				,
			}
		});
		return Task.CompletedTask;
	}

	public class CheckboxGroup
	{
		public CheckboxGroup(string name)
		{
			Name = name;
		}

		public string Name { get; set; }

		public List<HostAccountPermissionCheckboxModel> Items { get; set; } = new();
	}
}

public partial class HostAccountPermissionCheckboxModel : ObservableObject
{
	[ObservableProperty]
	private bool _checked;

	[ObservableProperty]
	private string? _text;
}