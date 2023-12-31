﻿using Amusoft.PCR.Domain.UI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Amusoft.PCR.Int.UI.DAL.Database;

internal class UiDbContext : DbContext
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public UiDbContext(DbContextOptions options) : base(options)
	{
	}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<LogEntry>().HasNoKey();
		modelBuilder.Entity<LogEntry>().Property(d => d.Time).HasConversion<DateTimeToTicksConverter>();

		modelBuilder.Entity<Endpoint>().HasIndex(d => d.Address).IsUnique();

		modelBuilder.Entity<EndpointAccount>().HasIndex(d => d.Email);
		modelBuilder.Entity<EndpointAccount>().HasIndex(nameof(EndpointAccount.EndpointId), nameof(EndpointAccount.Email)).IsUnique();
		
		if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
		{
			// SQLite does not have proper support for DateTimeOffset via Entity Framework Core, see the limitations
			// here: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
			// To work around this, when the Sqlite database provider is used, all model properties of type DateTimeOffset
			// use the DateTimeOffsetToBinaryConverter
			// Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
			// This only supports millisecond precision, but should be sufficient for most use cases.
			foreach (var entityType in modelBuilder.Model.GetEntityTypes())
			{
				var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset)
				                                                               || p.PropertyType == typeof(DateTimeOffset?));
				foreach (var property in properties)
				{
					modelBuilder
						.Entity(entityType.Name)
						.Property(property.Name)
						.HasConversion(new DateTimeOffsetToBinaryConverter());
				}
			}
		}
	}

	~UiDbContext()
	{
	}

	public override void Dispose()
	{
		base.Dispose();
	}

	public DbSet<BearerToken> BearerTokens { get; set; }

	public DbSet<LogEntry> LogEntries { get; set; }

	public DbSet<Endpoint> Endpoints { get; set; }

	public DbSet<EndpointAccount> EndpointAccounts { get; set; }
}