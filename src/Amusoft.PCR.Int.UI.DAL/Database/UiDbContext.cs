#region

using Amusoft.PCR.Domain.UI.Entities;
using Microsoft.EntityFrameworkCore;

#endregion

namespace Amusoft.PCR.Int.UI.DAL.Database;

internal class UiDbContext : DbContext
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public UiDbContext(DbContextOptions options) : base(options)
	{
	}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	public DbSet<BearerToken> BearerTokens { get; set; }
}