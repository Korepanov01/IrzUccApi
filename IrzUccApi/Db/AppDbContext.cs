using IrzUccApi.Models.Db;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi.Db
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string,
        IdentityUserClaim<string>, AppUserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        private const string SuperAdminEmail = "user@example.com";
        private const string SuperAdminPassword = "string";
        private const string SuperAdminName = "Главный";
        private const string SuperAdminSurname = "Администратор";

        public DbSet<Image> Images { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Cabinet> Cabinets { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<NewsEntry> NewsEntries { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<UserPosition> UserPositions { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            DbConfigurer.Configure(builder);
            SeedData(builder);
        }
        private static void SeedData(ModelBuilder builder)
        {
            var appRoles = new[] {
                new AppRole
                {
                    Name = Enums.RolesNames.SuperAdmin,
                    NormalizedName = Enums.RolesNames.SuperAdmin.ToUpper()
                },
                new AppRole
                {
                    Name = Enums.RolesNames.Admin,
                    NormalizedName = Enums.RolesNames.Admin.ToUpper()
                },
                new AppRole
                {
                    Name = Enums.RolesNames.Support,
                    NormalizedName = Enums.RolesNames.Support.ToUpper()
                },
                new AppRole
                {
                    Name = Enums.RolesNames.CabinetsManager,
                    NormalizedName = Enums.RolesNames.CabinetsManager.ToUpper()
                }
            };
            builder.Entity<AppRole>().HasData(appRoles);

            var superAdmin = new AppUser
            {
                FirstName = SuperAdminName,
                Surname = SuperAdminSurname,
                UserName = SuperAdminEmail,
                NormalizedUserName = SuperAdminEmail.ToUpper(),
                Email = SuperAdminEmail,
                NormalizedEmail = SuperAdminEmail.ToUpper()
            };
            superAdmin.PasswordHash = new PasswordHasher<AppUser>().HashPassword(superAdmin, SuperAdminPassword);
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
