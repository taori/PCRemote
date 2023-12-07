using Amusoft.PCR.Domain.Service.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Amusoft.PCR.Int.Service.Authorization;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<RefreshToken>()
			.HasKey(d => new { d.UserId, d.RefreshTokenId });

		builder.Entity<Permission>()
			.HasKey(d => new { d.UserId, d.SubjectId, d.PermissionType });
	}

	public DbSet<RefreshToken> RefreshTokens { get; set; }

	public DbSet<Permission> Permissions { get; set; }

	public DbSet<HostCommand> HostCommands { get; set; }

	public DbSet<VoiceRecognitionNode> AudioFeeds { get; set; }

	public DbSet<VoiceRecognitionNodeAlias> AudioFeedAliases { get; set; }

	public DbSet<VoiceRecognitionConfigurationPair> VoiceRecognitionConfiguration { get; set; }
}