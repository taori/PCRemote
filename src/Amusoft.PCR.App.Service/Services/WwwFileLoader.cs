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
#if DEBUG
		if (File.Exists(GetFullWWWPath("app.apk")))
		{
			return DownloadAsync( "PC Remote 3.apk", "app.apk", "application/vnd.android.package-archive");
		}
		else
		{
			return GetTestFile();
		}
#else
		
		return DownloadAsync( "PC Remote 3.apk", "app.apk", "application/vnd.android.package-archive");
#endif
	}

	private IResult DownloadAsync(string downloadName, string wwwRootPath, string contentType = "application/octet-stream")
	{
		var fullpath = GetFullWWWPath(wwwRootPath);

		_logger.LogInformation("Downloading file {Path} as {Name} with ContentType {ContentType}", fullpath, downloadName, contentType);
		
		return Results.File(fullpath, contentType, downloadName);
	}

	private static string GetFullWWWPath(string wwwRootPath)
	{
		return $@"{Directory.GetCurrentDirectory()}\wwwroot\{wwwRootPath}";
	}
}

public interface IWwwFileLoader
{
	IResult GetTestFile();
	IResult GetAndroidApp();
}