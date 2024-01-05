using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;
using Keyboard = Amusoft.PCR.Domain.UI.ValueTypes.Keyboard;

namespace Amusoft.PCR.Int.UI.Shared;

internal partial class UserInterfaceService : IUserInterfaceService
{
	public Task<string?> GetPromptTextAsync(string title, string message, string? acceptText, string? cancelText, string? placeholder, int? maxLength, string? initialValue, Keyboard keyboard = Keyboard.Default)
	{
		return MainThread.InvokeOnMainThreadAsync(() => Shell.Current.DisplayPromptAsync(title, message,
			acceptText ?? Translations.Generic_OK,
			cancelText ?? Translations.Generic_Cancel,
			placeholder,
			maxLength ?? -1,
			keyboard: GetMauiKeyboard(keyboard),
			initialValue: initialValue ?? string.Empty
		));
	}

	private Microsoft.Maui.Keyboard GetMauiKeyboard(Keyboard keyboard)
	{
		return keyboard switch
		{
			Keyboard.Default => Microsoft.Maui.Keyboard.Default,
			Keyboard.Plain => Microsoft.Maui.Keyboard.Plain,
			Keyboard.Chat => Microsoft.Maui.Keyboard.Chat,
			Keyboard.Email => Microsoft.Maui.Keyboard.Email,
			Keyboard.Numeric => Microsoft.Maui.Keyboard.Numeric,
			Keyboard.Telephone => Microsoft.Maui.Keyboard.Telephone,
			Keyboard.Text => Microsoft.Maui.Keyboard.Text,
			Keyboard.Url => Microsoft.Maui.Keyboard.Url,
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