using IrzUccApi.Db.Models;
using IrzUccApi.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi.Db
{
    static public class DbSeeder
    {
        static private readonly string _superAdminEmail = "user@example.com";
        static private readonly string _superAdminPassword = "string";
        static private readonly string _superAdminName = "Главный";
        static private readonly string _superAdminSurname = "Администратор";

        static public void SeedData(ModelBuilder builder)
        {
            Dictionary<string, AppRole> appRoles = new();

            foreach (var role in new[] { RolesNames.Admin, RolesNames.SuperAdmin, RolesNames.Support, RolesNames.CabinetsManager })
            {
                appRoles.Add(role, new AppRole
                {
                    Id = Guid.NewGuid(),
                    Name = role,
                    NormalizedName = role.ToUpper()
                });
            }
            builder.Entity<AppRole>().HasData(appRoles.Values);

            var superAdmin = new AppUser
            {
                Id = Guid.NewGuid(),
                FirstName = _superAdminName,
                Surname = _superAdminSurname,
                UserName = _superAdminEmail,
                NormalizedUserName = _superAdminEmail.ToUpper(),
                Email = _superAdminEmail,
                NormalizedEmail = _superAdminEmail.ToUpper(),
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            superAdmin.PasswordHash = new PasswordHasher<AppUser>().HashPassword(superAdmin, _superAdminPassword);
            builder.Entity<AppUser>().HasData(superAdmin);

            var superAdminUserRoles = appRoles.Select(r => new
            {
                Id = Guid.NewGuid(),
                RoleId = r.Value.Id,
                UserId = superAdmin.Id,
            });
            builder.Entity<AppUserRole>().HasData(superAdminUserRoles);
        }
    }
}