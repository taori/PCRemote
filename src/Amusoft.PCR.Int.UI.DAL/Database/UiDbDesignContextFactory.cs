#region

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

#endregion

namespace Amusoft.PCR.Int.UI.DAL.Database;

internal class UiDbDesignContextFactory : IDesignTimeDbContextFactory<UiDbContext>
{
	public UiDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<UiDbContext>();
		var targetDirectory = GetFolder();
		optionsBuilder.UseSqlite($"Data Source={targetDirectory}pcr3-designdb.db");
		return new UiDbContext(optionsBuilder.Options);
	}

	private static string GetFolder()
	{
		var solutionPath = FindSolutionInParent(4, Path.GetDirectoryName(Assembly.GetAssembly(typeof(UiDbDesignContextFactory))!.Location)!);
		if (!string.IsNullOrEmpty(solutionPath))
			return $"{Path.GetDirectoryName(solutionPath)}\\Amusoft.PCR.Int.UI.DAL\\Resources\\";

		throw new Exception("Solution file not found");
	}

	private static string? FindSolutionInParent(int maxParentJumps, string directoryName)
	{
		var match = Directory.EnumerateFiles(directoryName, "All.sln").FirstOrDefault();
		if (!string.IsNullOrEmpty(match))
			return match;

		if (maxParentJumps > 0)
			return FindSolutionInParent(--maxParentJumps, Path.GetDirectoryName(directoryName)!);

		return null;
	}
}