﻿using Amusoft.PCR.AM.Service.Interfaces;
using Amusoft.PCR.AM.Service.Utility;
using Amusoft.PCR.Domain.Service.Entities;
using Amusoft.PCR.Int.Service.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Amusoft.PCR.Int.Service.Services;

public class ApplicationDbSeedService : BackgroundService
{
	private readonly IServiceScopeFactory _serviceScopeFactory;
	private readonly IEnumerable<IRoleNameProvider> _roleNameProviders;
	private readonly ILogger<ApplicationDbSeedService> _logger;
	private readonly IApplicationStateTransmitter _applicationStateTransmitter;

	public ApplicationDbSeedService(IServiceScopeFactory serviceScopeFactory,
		IEnumerable<IRoleNameProvider> roleNameProviders,
		ILogger<ApplicationDbSeedService> logger,
		IApplicationStateTransmitter applicationStateTransmitter)
	{
		_serviceScopeFactory = serviceScopeFactory;
		_roleNameProviders = roleNameProviders;
		_logger = logger;
		_applicationStateTransmitter = applicationStateTransmitter;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogTrace("Waiting for configuration to be done");
		await _applicationStateTransmitter.ConfigurationDone;

		_logger.LogTrace("{Name} running", nameof(ApplicationDbSeedService));

		using (var serviceScope = _serviceScopeFactory.CreateScope())
		{
			using var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

			_logger.LogDebug("Adding roles from {Name} implementations", nameof(IRoleNameProvider));
			foreach (var provider in _roleNameProviders)
			{
				_logger.LogTrace("Adding roles from {Type}", provider.GetType().Name);
				foreach (var roleName in provider.GetRoleNames())
				{
					await EnsureRoleExistsAsync(roleName, roleManager);
				}
			}

			await EnsureAdminsHavePermissionsAsync(serviceScope.ServiceProvider);

			await AddHostCommandsAsync(serviceScope.ServiceProvider);
		}

		_logger.LogTrace("{Name} complete", nameof(ApplicationDbSeedService));
	}

	private async Task AddHostCommandsAsync(IServiceProvider serviceProvider)
	{
		var hostCommandService = serviceProvider.GetRequiredService<IHostCommandService>();
		var allCommands = await hostCommandService.GetAllAsync();
		if (allCommands.All(d => d.ProgramPath != "spotify"))
		{
			await hostCommandService.CreateAsync(new HostCommand() { ProgramPath = "spotify", CommandName = "Spotify" });
		}

		if (!allCommands.Any(d => d.ProgramPath == "explorer" && d.Arguments == "https://www.google.com"))
		{
			await hostCommandService.CreateAsync(new HostCommand() { ProgramPath = "explorer", Arguments = "https://www.google.com", CommandName = "Browser" });
		}
	}

	private async Task EnsureAdminsHavePermissionsAsync(IServiceProvider serviceProvider)
	{
		var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
		var adminUsers = await userManager.Users.Where(d => d.UserType == UserType.Administrator).ToListAsync();
		foreach (var applicationUser in adminUsers)
		{
			if (await userManager.IsInRoleAsync(applicationUser, RoleNames.Administrator))
				continue;

			_logger.LogWarning("User {Name} is missing administrator role - granting permission", applicationUser.UserName);
			await userManager.AddToRoleAsync(applicationUser, RoleNames.Administrator);
		}
	}

	private async Task EnsureRoleExistsAsync(string roleName, RoleManager<IdentityRole> roleManager)
	{
		_logger.LogTrace("Ensuring that permission {Name} exists", roleName);
		var role = await roleManager.FindByNameAsync(roleName);
		if (role == null)
		{
			_logger.LogDebug("Creating role {Role}", roleName);
			await roleManager.CreateAsync(new IdentityRole(roleName));
		}
		else
		{
			_logger.LogDebug("Role {Role} already exists", roleName);
		}
	}
}