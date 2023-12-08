using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Amusoft.PCR.Int.Service.Authorization;

#if DEBUG

public class DesignDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
	public ApplicationDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
		var targetDirectory = GetFolder();
		optionsBuilder.UseSqlite($"Data Source={targetDirectory}pcr3-designdb.db");

		return new ApplicationDbContext(optionsBuilder.Options);
	}

	public static void TestMethod()
	{
		var folder = GetFolder();
	}

	private static string GetFolder()
	{
		var solutionPath = FindSolutionInParent(4, Path.GetDirectoryName(Assembly.GetAssembly(typeof(DesignDbContextFactory))!.Location)!);
		if (!string.IsNullOrEmpty(solutionPath))
			return $"{Path.GetDirectoryName(solutionPath)}\\Amusoft.PCR.Int.Service\\Resources\\";
		
		throw new Exception("Solution file not found");
	}

	private static string? FindSolutionInParent(int maxParentJumps, string directoryName)
	{
		var match = Directory.EnumerateFiles(directoryName, "All.sln").FirstOrDefault();
		if (!string.IsNullOrEmpty(match))
			return match;

		if (maxParentJumps > 0)
			return FindSolutionInParent(--maxParentJumps, Path.GetDirectoryName(directoryName));

		return null;
	}
}

#endif