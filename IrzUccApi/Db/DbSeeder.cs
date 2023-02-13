using IrzUccApi.Enums;
using IrzUccApi.Models.Db;
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
            var appRoles = new List<AppRole>();
            foreach (var role in new[] { RolesNames.Admin, RolesNames.SuperAdmin, RolesNames.Support, RolesNames.CabinetsManager })
            {
                appRoles.Add(new AppRole
                {
                    Name = role,
                    NormalizedName = role.ToUpper()
                });
            }
            builder.Entity<AppRole>().HasData(appRoles);

            var superAdmin = new AppUser
            {
                FirstName = _superAdminName,
                Surname = _superAdminSurname,
                UserName = _superAdminEmail,
                NormalizedUserName = _superAdminEmail.ToUpper(),
                Email = _superAdminEmail,
                NormalizedEmail = _superAdminEmail.ToUpper(),
            };
            superAdmin.PasswordHash = new PasswordHasher<AppUser>().HashPassword(superAdmin, _superAdminPassword);
            builder.Entity<AppUser>().HasData(superAdmin);

            var superAdminUserRoles = appRoles.Select(r => new AppUserRole
            {
                RoleId = r.Id,
                UserId = superAdmin.Id
            });
            builder.Entity<AppUserRole>().HasData(superAdminUserRoles);
        }
    }
}