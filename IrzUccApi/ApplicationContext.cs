using IrzUccApi.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi
{
    public class ApplicationContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<NewsEntry> NewsEntries { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<PositionHistoricalRecord> PositionHistoricalRecords { get; set; }
        public DbSet<Role> Roles { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>()
                .HasMany<User>(e => e.Listeners)
                .WithMany(u => u.ListeningEvents);
            modelBuilder.Entity<Event>()
                .HasOne<User>(e => e.Creator)
                .WithMany(u => u.MyEvents);
            modelBuilder.Entity<User>()
                .HasMany<NewsEntry>(u => u.LikedNewsEntries)
                .WithMany(n => n.Likers);
            modelBuilder.Entity<User>()
                .HasMany<NewsEntry>(u => u.MyNewsEntries)
                .WithOne(n => n.Author);
            modelBuilder.Entity<User>()
                .HasMany<User>(u => u.Subscribers)
                .WithMany(u => u.Subscriptions);
        }
    }
}
