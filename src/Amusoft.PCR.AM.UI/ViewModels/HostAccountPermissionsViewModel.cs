using System.Windows.Input;
using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using Amusoft.PCR.Domain.Shared.Entities;
using Amusoft.PCR.Domain.UI.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class HostAccountPermissionsViewModel : ReloadablePageViewModel, INavigationCallbacks
{
	private readonly EndpointAccount _endpointAccount;
	private readonly IIpcIntegrationService _ipcIntegrationService;

	public HostAccountPermissionsViewModel(
		ITypedNavigator navigator
		, EndpointAccount endpointAccount
		, IIpcIntegrationService ipcIntegrationService) : base(navigator)
	{
		_endpointAccount = endpointAccount;
		_ipcIntegrationService = ipcIntegrationService;
	}

	[ObservableProperty]
	private List<CheckboxGroup> _items = new();

	protected override string GetDefaultPageTitle()
	{
		return Translations.HostAccountPermissions_Title;
	}

	public Task OnNavigatedToAsync()
	{
		return ReloadAsync();
	}

	protected override async Task OnReloadAsync(CancellationToken cancellationToken)
	{
		var permissions = await _ipcIntegrationService.IdentityExtendedClient.GetPermissionsAsync(_endpointAccount.Email, CancellationToken.None);
		Items = new List<CheckboxGroup>()
		{
			new(Translations.HostAccountPermissions_Roles, GetRoleCheckboxes(permissions)), new(Translations.HostAccountPermissions_UserPermissions, GetUserPermissionCheckboxes(permissions))
		};
	}

	private IEnumerable<HostAccountPermissionCheckboxModel> GetUserPermissionCheckboxes(UserPermissionSet? permissions)
	{
		if (permissions is null)
			yield break;

		foreach (var permission in permissions.Permissions)
		{
			yield return new HostAccountPermissionCheckboxModel() { Text = permission.Name, Checked = permission.Granted };
		}
	}

	private static IEnumerable<HostAccountPermissionCheckboxModel> GetRoleCheckboxes(UserPermissionSet? permissions)
	{
		if (permissions is null)
			yield break;

		foreach (var role in permissions.Roles)
		{
			if (string.Equals(RoleNames.Administrator, role.Name, StringComparison.OrdinalIgnoreCase))
				continue;

			yield return new HostAccountPermissionCheckboxModel() { Text = role.Name, Checked = role.Granted };
		}
	}

	public class CheckboxGroup : List<HostAccountPermissionCheckboxModel>
	{
		public CheckboxGroup(string name, IEnumerable<HostAccountPermissionCheckboxModel> items) : base(items)
		{
			Name = name;
		}

		public string Name { get; set; }
	}
}

public partial class HostAccountPermissionCheckboxModel : ObservableObject
{
	public HostAccountPermissionCheckboxModel()
	{
		_toggleCheckedCommand = new RelayCommand(() => Checked = !_checked);
	}

	[ObservableProperty]
	private ICommand _toggleCheckedCommand;
	
	[ObservableProperty]
	private bool _checked;

	[ObservableProperty]
	private string? _text;
}