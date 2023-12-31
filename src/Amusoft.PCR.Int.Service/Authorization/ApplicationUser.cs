﻿using Amusoft.PCR.Domain.Shared.ValueTypes;
using Microsoft.AspNetCore.Identity;

namespace Amusoft.PCR.Int.Service.Authorization;

public class ApplicationUser : IdentityUser
{
	public UserType UserType { get; set; }

	public DateTime? LastSignIn { get; set; }
}