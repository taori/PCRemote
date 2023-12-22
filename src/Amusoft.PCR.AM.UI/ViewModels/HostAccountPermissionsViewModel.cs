using System.Windows.Input;
using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;
using Amusoft.PCR.AM.UI.ViewModels.Shared;
using Amusoft.PCR.Domain.Shared.Entities;
using Amusoft.PCR.Domain.Shared.ValueTypes;
using Amusoft.PCR.Domain.UI.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Amusoft.PCR.AM.UI.ViewModels;

public partial class HostAccountPermissionsViewModel : ReloadablePageViewModel, INavigationCallbacks
{
	private readonly IUserInterfaceService _userInterfaceService;
	private readonly EndpointAccount _endpointAccount;
	private readonly IIpcIntegrationService _ipcIntegrationService;

	[ObservableProperty]
	private List<CheckboxGroup> _items = new();

	private List<CheckboxGroup> _changeBaseline = new();

	public HostAccountPermissionsViewModel(
		ITypedNavigator navigator,
		IUserInterfaceService userInterfaceService,
		EndpointAccount endpointAccount,
		IIpcIntegrationService ipcIntegrationService) : base(navigator)
	{
		_userInterfaceService = userInterfaceService;
		_endpointAccount = endpointAccount;
		_ipcIntegrationService = ipcIntegrationService;
	}

	public Task OnNavigatedToAsync()
	{
		return ReloadAsync();
	}

	public async Task OnNavigatingAsync(INavigatingContext context)
	{
		await using var pause = context.PauseNavigation();
		var hasChanges = false;
		for (var i = 0; i < _changeBaseline.Count; i++)
		{
			hasChanges = hasChanges || !_changeBaseline[i].Equals(_items[i]);
		}

		if (hasChanges && await _userInterfaceService.DisplayConfirmAsync(Translations.Generic_Question, Translations.Generic_SaveChangesRequest))
		{
			await SaveChangesAsync(_items);
		}
	}

	private async Task SaveChangesAsync(List<CheckboxGroup> items)
	{
		var groupByCategory = items
			.SelectMany(d => d.Select(item => (item.Type, f: item)))
			.ToLookup(d => d.Type, d => d.f);
		var permissions = new UserPermissionSet()
		{
			Permissions = ReadPermissions(groupByCategory[HostAccountPermissionCheckboxModel.Category.UserPermissions]),
			Roles = ReadRoles(groupByCategory[HostAccountPermissionCheckboxModel.Category.Roles]),
			UserId = _endpointAccount.Id.ToString()
		};
		await _ipcIntegrationService.IdentityExtendedClient.UpdatePermissionsAsync(_endpointAccount.Email, permissions, CancellationToken.None);
	}

	private UserRole[] ReadRoles(IEnumerable<HostAccountPermissionCheckboxModel> items)
	{
		return GetItems().ToArray();

		IEnumerable<UserRole> GetItems()
		{
			foreach (var item in items)
			{
				if (item.Id is HostAccountPermissionCheckboxModel.RoleId roleId)
				{
					yield return new UserRole()
					{
						Granted = item.Checked,
						Id = roleId.Id,
						Name = item.Text,
					};
				}
			}
		}
	}

	private UserPermission[] ReadPermissions(IEnumerable<HostAccountPermissionCheckboxModel> items)
	{
		return Array.Empty<UserPermission>();
	}

	protected override string GetDefaultPageTitle()
	{
		return Translations.HostAccountPermissions_Title;
	}

	protected override async Task OnReloadAsync(CancellationToken cancellationToken)
	{
		var permissions = await _ipcIntegrationService.IdentityExtendedClient.GetPermissionsAsync(_endpointAccount.Email, CancellationToken.None);
		Items = new List<CheckboxGroup>()
		{
			new(Translations.HostAccountPermissions_Roles, GetRoleCheckboxes(permissions)),
			new(Translations.HostAccountPermissions_UserPermissions, GetUserPermissionCheckboxes(permissions))
		};
		_changeBaseline = new List<CheckboxGroup>()
		{
			new(Translations.HostAccountPermissions_Roles, GetRoleCheckboxes(permissions)),
			new(Translations.HostAccountPermissions_UserPermissions, GetUserPermissionCheckboxes(permissions))
		};
	}

	private IEnumerable<HostAccountPermissionCheckboxModel> GetUserPermissionCheckboxes(UserPermissionSet? permissions)
	{
		if (permissions is null)
			yield break;

		foreach (var permission in permissions.Permissions)
		{
			yield return new HostAccountPermissionCheckboxModel(
				new HostAccountPermissionCheckboxModel.UserPermissionId((permission.PermissionType, permission.SubjectId, permission.Name)),
				HostAccountPermissionCheckboxModel.Category.UserPermissions
			) { Text = permission.Name, Checked = permission.Granted };
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

			yield return new HostAccountPermissionCheckboxModel(
				new HostAccountPermissionCheckboxModel.RoleId(role.Id),
				HostAccountPermissionCheckboxModel.Category.Roles
			)
			{
				Text = role.Name,
				Checked = role.Granted
			};
		}
	}

	public class CheckboxGroup : List<HostAccountPermissionCheckboxModel>, IEquatable<CheckboxGroup>
	{
		public CheckboxGroup(string name, IEnumerable<HostAccountPermissionCheckboxModel> items) : base(items)
		{
			Name = name;
		}

		public string Name { get; set; }

		public bool Equals(CheckboxGroup? other)
		{
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			if (Count != other.Count)
				return false;
			if (Name != other.Name)
				return false;
			for (var i = 0; i < Count; i++)
			{
				if (!this[i].Equals(other[i]))
					return false;
			}

			return true;
		}

		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != this.GetType())
				return false;
			return Equals((CheckboxGroup)obj);
		}

		public override int GetHashCode()
		{
			var code = 0;
			for (var i = 0; i < Count; i++)
			{
				code = HashCode.Combine(code, this[i]);
			}

			code = HashCode.Combine(code, Name.GetHashCode());
			return code;
		}
	}
}

public partial class HostAccountPermissionCheckboxModel : ObservableObject, IEquatable<HostAccountPermissionCheckboxModel>
{
	public enum Category
	{
		Roles,
		UserPermissions
	}

	public readonly CheckboxId Id;

	public readonly Category Type;

	[ObservableProperty]
	private bool _checked;

	[ObservableProperty]
	private string? _text;

	[ObservableProperty]
	private ICommand _toggleCheckedCommand;

	public HostAccountPermissionCheckboxModel(CheckboxId id, Category type)
	{
		Id = id;
		Type = type;
		_toggleCheckedCommand = new RelayCommand(() => Checked = !_checked);
	}

	public abstract class CheckboxId
	{
		public abstract bool Equals(CheckboxId value);
		public abstract int GetHashCode();
	}

	public abstract class CheckboxId<TId> : CheckboxId
	{
		public CheckboxId(TId id)
		{
			Id = id;
		}

		public TId Id { get; }

		public override bool Equals(CheckboxId value)
		{
			if (value is CheckboxId<TId> c)
				return HashCode.Combine(Id) == HashCode.Combine(c.Id);

			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Id);
		}
	}

	public class RoleId : CheckboxId<string>
	{
		public RoleId(string id) : base(id)
		{
		}
	}

	public class UserPermissionId : CheckboxId<(PermissionKind Kind, string SubjectId, string Name)>
	{
		public UserPermissionId((PermissionKind Kind, string SubjectId, string Name) id) : base(id)
		{
		}
	}

	public bool Equals(HostAccountPermissionCheckboxModel? other)
	{
		if (ReferenceEquals(null, other))
			return false;
		if (ReferenceEquals(this, other))
			return true;
		return Id.Equals(other.Id) && Type == other.Type && _checked == other._checked;
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj))
			return false;
		if (ReferenceEquals(this, obj))
			return true;
		if (obj.GetType() != this.GetType())
			return false;
		return Equals((HostAccountPermissionCheckboxModel)obj);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Id, (int)Type, _checked);
	}
}