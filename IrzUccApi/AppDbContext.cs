using IrzUccApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi
{
    public class AppDbContext: IdentityDbContext<AppUser>
    {
        static public readonly string SuperAdminRole = "SuperAdmin";
        static public readonly string AdminRole = "Admin";
        static public readonly string SupportRole = "Support";
        static public readonly string СhancelleryRole = "Сhancellery";

        public DbSet<Chat> Chats { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<NewsEntry> NewsEntries { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<PositionHistoricalRecord> PositionHistoricalRecords { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureModel(builder);
            SeedData(builder);
        }

        private void ConfigureModel(ModelBuilder builder)
        {
            builder.Entity<Event>()
               .HasMany<AppUser>(e => e.Listeners)
               .WithMany(u => u.ListeningEvents)
               .UsingEntity(join => join.ToTable("EventListening"));
            builder.Entity<Event>()
                .HasOne<AppUser>(e => e.Creator)
                .WithMany(u => u.MyEvents);
            builder.Entity<AppUser>()
                .HasMany<NewsEntry>(u => u.LikedNewsEntries)
                .WithMany(n => n.Likers)
                .UsingEntity(join => join.ToTable("Like"));
            builder.Entity<AppUser>()
                .HasMany<NewsEntry>(u => u.MyNewsEntries)
                .WithOne(n => n.Author);
            builder.Entity<AppUser>()
                .HasMany<AppUser>(u => u.Subscribers)
                .WithMany(u => u.Subscriptions)
                .UsingEntity(join => join.ToTable("Subscription"));
        }

        private void SeedData(ModelBuilder builder)
        {
            var adminUserId = Guid.NewGuid().ToString();
            var superAdminRoleId = Guid.NewGuid().ToString();

            builder.Entity<IdentityRole>().HasData(new[] {
                new IdentityRole
                {
                    Id = superAdminRoleId,
                    Name = SuperAdminRole,
                    NormalizedName = SuperAdminRole.ToUpper()
                },
                new IdentityRole
                {
                    Name = AdminRole,
                    NormalizedName = AdminRole.ToUpper()
                },
                new IdentityRole
                {
                    Name = SupportRole,
                    NormalizedName = SupportRole.ToUpper()
                },
                new IdentityRole
                {
                    Name = СhancelleryRole,
                    NormalizedName = СhancelleryRole.ToUpper()
                }
            });

            var superAdmin = new AppUser
            {
                Id = adminUserId,
                Email = "user@example.com"
            };
            superAdmin.PasswordHash = new PasswordHasher<AppUser>().HashPassword(superAdmin, "string");
            builder.Entity<AppUser>().HasData(superAdmin);

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = superAdminRoleId,
                UserId = adminUserId
            });
        }
    }
}
