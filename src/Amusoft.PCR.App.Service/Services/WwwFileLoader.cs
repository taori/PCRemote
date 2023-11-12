namespace Amusoft.PCR.App.Service.Services;

public class WwwFileLoader : IWwwFileLoader
{
	private readonly ILogger<WwwFileLoader> _logger;

	public WwwFileLoader(ILogger<WwwFileLoader> logger)
	{
		_logger = logger;
	}

	public async Task WriteTestAsync(HttpContext context)
	{
		await DownloadAsync(context, "test.txt", "downloadtest.txt");
	}

	public async Task WriteAndroidAsync(HttpContext context)
	{
		await DownloadAsync(context, "PC Remote 3.apk", "app.apk", "application/vnd.android.package-archive");
	}

	private async Task DownloadAsync(HttpContext context, string downloadName, string wwwRootPath, string contentType = "application/octet-stream")
	{
		var fullpath = $@"{Directory.GetCurrentDirectory()}\wwwroot\{wwwRootPath}";

		_logger.LogInformation("Downloading file {Path} as {Name} with ContentType {ContentType}", fullpath, downloadName, contentType);

		context.Response.Headers.Add("Content-Type", contentType);
		context.Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{downloadName}\"");

		var fileContent = await File.ReadAllBytesAsync(fullpath);

		await context.Response.Body.WriteAsync(fileContent);
	}
}

public interface IWwwFileLoader
{
	Task WriteTestAsync(HttpContext context);
	Task WriteAndroidAsync(HttpContext context);
}