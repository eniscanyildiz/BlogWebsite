﻿using System;
using Microsoft.AspNetCore.Identity;

namespace BlogProject.Models
{
	public class ApplicationUser: IdentityUser
	{
        public string FullName { get; set; }
    }
}

