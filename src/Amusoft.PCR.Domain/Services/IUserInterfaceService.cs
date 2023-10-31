using System.Threading.Channels;

namespace Amusoft.PCR.Domain.Services;

public interface IUserInterfaceService
{
	Task<string?> GetPromptText(
		string title, 
		string message, 
		string? acceptText = null, 
		string? cancelText = null, 
		string? placeholder = null, 
		int? maxLength = null, 
		string? initialValue = null);

	Task<bool> DisplayConfirmAsync(string title, string message, string? acceptText = null, string? cancelText = null);

	Task DisplayAlert(string title, string message, string? acceptText = null);
}