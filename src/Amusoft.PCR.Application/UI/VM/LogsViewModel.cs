using Amusoft.PCR.Application.Services;
using Amusoft.PCR.Application.Shared;
using Amusoft.PCR.Domain.VM;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Amusoft.PCR.Application.UI.VM;

public partial class LogsViewModel : ReloadablePageViewModel, INavigationCallbacks
{
	public LogsViewModel(ITypedNavigator navigator) : base(navigator)
	{
	}

	[ObservableProperty]
	private string? _text;

	public Task OnNavigatedToAsync()
	{
		return ReloadAsync();
	}

	private async Task<string?> GetTextAsync()
	{
		var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		var path = Path.Combine(root, "logs", "nlog.csv");

		return File.Exists(path)
			? await ReadFileContentAsync(path)
			: null;

		static async Task<string> ReadFileContentAsync(string path)
		{
			await using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			using var reader = new StreamReader(fileStream);
			return await reader.ReadToEndAsync();
		}
	}

	protected override string GetDefaultPageTitle()
	{
		return "Logs";
	}

	protected override async Task OnReloadAsync(CancellationToken cancellationToken)
	{
		Text = await GetTextAsync();
	}
}