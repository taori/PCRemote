using Amusoft.PCR.Domain.UI.ValueTypes;

namespace Amusoft.PCR.AM.UI.Interfaces;

public interface IUserInterfaceService
{
	Task<string?> GetPromptText(
		string title, 
		string message, 
		string? acceptText = null, 
		string? cancelText = null, 
		string? placeholder = null, 
		int? maxLength = null, 
		string? initialValue = null,
		Keyboard keyboard = Keyboard.Default);

	Task<bool> DisplayConfirmAsync(string title, string message, string? acceptText = null, string? cancelText = null);

	Task DisplayAlert(string title, string message, string? acceptText = null);
}