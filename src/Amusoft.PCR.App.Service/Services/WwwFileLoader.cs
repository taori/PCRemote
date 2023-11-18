namespace Amusoft.PCR.App.Service.Services;

public class WwwFileLoader : IWwwFileLoader
{
	private readonly ILogger<WwwFileLoader> _logger;

	public WwwFileLoader(ILogger<WwwFileLoader> logger)
	{
		_logger = logger;
	}

	public IResult GetTestFile()
	{
		return DownloadAsync( "test.txt", "downloadtest.txt");
	}

	public IResult GetAndroidApp()
	{
		return DownloadAsync( "PC Remote 3.apk", "app.apk", "application/vnd.android.package-archive");
	}

	private IResult DownloadAsync(string downloadName, string wwwRootPath, string contentType = "application/octet-stream")
	{
		var fullpath = $@"{Directory.GetCurrentDirectory()}\wwwroot\{wwwRootPath}";

		_logger.LogInformation("Downloading file {Path} as {Name} with ContentType {ContentType}", fullpath, downloadName, contentType);
		
		return Results.File(fullpath, contentType, downloadName);
	}
}

public interface IWwwFileLoader
{
	IResult GetTestFile();
	IResult GetAndroidApp();
}