﻿using Amusoft.PCR.AM.Shared.Resources;
using Amusoft.PCR.AM.UI.Interfaces;

namespace Amusoft.PCR.Int.UI.Shared;

internal class UserInterfaceService : IUserInterfaceService
{
	public Task<string?> GetPromptText(string title, string message, string? acceptText, string? cancelText, string? placeholder, int? maxLength, string? initialValue)
	{
		return Shell.Current.DisplayPromptAsync(title, message,
			acceptText ?? Translations.Generic_OK,
			cancelText ?? Translations.Generic_Cancel,
			placeholder,
			maxLength ?? -1,
			initialValue: initialValue ?? string.Empty
		);
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

	public Task DisplayAlert(string title, string message, string? acceptText = null)
	{
		return Shell.Current.DisplayAlert(
			title,
			message,
			acceptText ?? Translations.Generic_OK,
			FlowDirection.MatchParent
		);
	}
}