namespace Amusoft.PCR.Domain.Services;

public interface IFileStorage
{
	Task WriteAsync(string path, byte[] bytes, CancellationToken cancellationToken);
	Task WriteJsonAsync<T>(string path, T value, CancellationToken cancellationToken);
	Task<byte[]> ReadAsync(string path, CancellationToken cancellationToken);
	Task<T> ReadJsonAsync<T>(string path, CancellationToken cancellationToken);
	bool PathExists(string path);
}