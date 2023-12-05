using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;

namespace Amusoft.PCR.Int.UI.Shared;

internal partial class UserInterfaceService : IUserInterfaceService
{
	public Task<string?> GetPromptTextAsync(string title, string message, string? acceptText, string? cancelText, string? placeholder, int? maxLength, string? initialValue, Domain.UI.ValueTypes.Keyboard keyboard = Domain.UI.ValueTypes.Keyboard.Default)
	{
		return Shell.Current.DisplayPromptAsync(title, message,
			acceptText ?? Translations.Generic_OK,
			cancelText ?? Translations.Generic_Cancel,
			placeholder,
			maxLength ?? -1,
			keyboard: GetMauiKeyboard(keyboard),
			initialValue: initialValue ?? string.Empty
		);
	}

	private Keyboard GetMauiKeyboard(Domain.UI.ValueTypes.Keyboard keyboard)
	{
		return keyboard switch
		{
			Domain.UI.ValueTypes.Keyboard.Default => Keyboard.Default,
			Domain.UI.ValueTypes.Keyboard.Plain => Keyboard.Plain,
			Domain.UI.ValueTypes.Keyboard.Chat => Keyboard.Chat,
			Domain.UI.ValueTypes.Keyboard.Email => Keyboard.Email,
			Domain.UI.ValueTypes.Keyboard.Numeric => Keyboard.Numeric,
			Domain.UI.ValueTypes.Keyboard.Telephone => Keyboard.Telephone,
			Domain.UI.ValueTypes.Keyboard.Text => Keyboard.Text,
			Domain.UI.ValueTypes.Keyboard.Url => Keyboard.Url,
			_ => throw new ArgumentOutOfRangeException(nameof(keyboard), keyboard, null)
		};
	}

	public Task<bool> DisplayConfirmAsync(string title, string message, string? acceptText = null, string? cancelText = null)
	{
		return Shell.Current.DisplayAlert(
			title,
			message,
			acceptText ?? Translations.Generic_OK,
			cancelText ?? Translations.Generic_Cancel, 
			FlowDirection.MatchParent
		);
	}

	public Task DisplayAlertAsync(string title, string message, string? acceptText = null)
	{
		return Shell.Current.DisplayAlert(
			title,
			message,
			acceptText ?? Translations.Generic_OK,
			FlowDirection.MatchParent
		);
	}
}