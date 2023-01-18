using IrzUccApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi
{
    public class AppDbContext: IdentityDbContext<AppUser>
    {
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<NewsEntry> NewsEntries { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<PositionHistoricalRecord> PositionHistoricalRecords { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
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
    }
}
