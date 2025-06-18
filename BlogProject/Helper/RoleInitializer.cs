using System;
using BlogProject.Models;
using Microsoft.AspNetCore.Identity;

namespace BlogProject.Helper
{
    public static class RoleInitializer
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            string adminRole = "Admin";
            string adminEmail = "enis@gmail.com";

            // Rol yoksa oluştur
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            // Admin kullanıcıyı bul ve role ata
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, adminRole))
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
    }
}

