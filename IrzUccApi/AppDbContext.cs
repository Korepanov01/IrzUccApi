using IrzUccApi.Models.Db;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi
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
        public DbSet<UserPosition> userPositions { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureModel(builder);
            SeedData(builder);
        }

        private static void ConfigureModel(ModelBuilder builder)
        {
            builder.Entity<Event>()
               .HasMany(e => e.Listeners)
               .WithMany(u => u.ListeningEvents)
               .UsingEntity(join => join.ToTable("EventListening"));

            builder.Entity<AppUser>()
                .HasMany(u => u.Comments)
                .WithOne(c => c.Author)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<AppUser>()
                .HasMany(u => u.Events)
                .WithOne(e => e.Creator)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<AppUser>()
                .HasMany(u => u.UserPosition)
                .WithOne(p => p.User)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<AppUser>()
                .HasMany(u => u.LikedNewsEntries)
                .WithMany(n => n.Likers)
                .UsingEntity(join => join.ToTable("Like"));
            builder.Entity<AppUser>()
                .HasMany(u => u.NewsEntries)
                .WithOne(n => n.Author)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<AppUser>()
                .HasMany(u => u.Subscribers)
                .WithMany(u => u.Subscriptions)
                .UsingEntity(join => join.ToTable("Subscription"));
            builder.Entity<AppUser>()
                .HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId);

            builder.Entity<AppUserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRole)
                .HasForeignKey(ur => ur.RoleId);
            builder.Entity<AppUserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            builder.Entity<Chat>()
                .HasMany(c => c.Participants)
                .WithMany(u => u.Chats)
                .UsingEntity(join => join.ToTable("ChatParticipants"));
            builder.Entity<Chat>()
                .HasMany(c => c.Messages)
                .WithOne(m => m.Chat)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Chat>()
                .HasOne(c => c.LastMessage)
                .WithOne();

            builder.Entity<NewsEntry>()
                .HasMany(n => n.Comments)
                .WithOne(c => c.NewsEntry)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Cabinet>()
                .HasMany(c => c.Events)
                .WithOne(e => e.Cabinet);
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
