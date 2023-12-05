using Amusoft.PCR.AM.UI.Interfaces;

namespace Amusoft.PCR.Int.UI.Shared;

internal partial class UserInterfaceService : IUserInterfaceService
{
	public Task<TimeSpan?> GetTimeFromPickerAsync(string title, TimeSpan intialTime)
	{
		return Task.FromResult((TimeSpan?)intialTime);
	}
}