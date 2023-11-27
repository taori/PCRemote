using System.Text.Json;
using Amusoft.PCR.AM.UI.Interfaces;

namespace Amusoft.PCR.App.UI.Implementations;

public class FileStorage : IFileStorage
{
	public Task WriteAsync(string path, byte[] bytes, CancellationToken cancellationToken)
	{
		return File.WriteAllBytesAsync(GetAppRootedPath(path), bytes, cancellationToken);
	}

	private static string GetAppRootedPath(string path)
	{
		return Path.Combine(FileSystem.Current.AppDataDirectory, path);
	}

	public async Task WriteJsonAsync<T>(string path, T value, CancellationToken cancellationToken)
	{
		var fullPath = GetAppRootedPath(path);
		await using (var fileStream = File.Exists(fullPath)
			             ? File.Open(fullPath, FileMode.Truncate)
			             : File.Create(fullPath))
		{
			await JsonSerializer.SerializeAsync(fileStream, value, cancellationToken: cancellationToken).ConfigureAwait(false);
		}
	}

	public Task<byte[]> ReadAsync(string path, CancellationToken cancellationToken)
	{
		return File.ReadAllBytesAsync(GetAppRootedPath(path), cancellationToken);
	}

	public async Task<T?> ReadJsonAsync<T>(string path, CancellationToken cancellationToken)
	{
		var fullPath = GetAppRootedPath(path);
		if (!Path.Exists(fullPath))
		{
			return default;
		}

		await using (var fileStream = File.OpenRead(fullPath))
		{
			return await JsonSerializer.DeserializeAsync<T>(fileStream, cancellationToken: cancellationToken).ConfigureAwait(false);
		}
	}

	public bool PathExists(string path)
	{
		return Path.Exists(GetAppRootedPath(path));
	}
}