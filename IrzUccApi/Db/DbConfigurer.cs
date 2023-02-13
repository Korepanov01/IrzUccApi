using IrzUccApi.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi.Db
{
    static public class DbConfigurer
    {
        private static readonly string _chatsParticipantsTableName = "ChatParticipants";
        private static readonly string _eventsListenersTableName = "EventListenings";
        private static readonly string _newsLikersTableName = "Likes";
        private static readonly string _subscribersSubscribtionsTableName = "Subscriptions";

        static public void Configure(ModelBuilder builder)
        {
            ConfigureEvent(builder);
            ConfigureAppUser(builder);
            ConfigureAppUserRole(builder);
            ConfigureChat(builder);
            ConfigureNewsEntry(builder);
            ConfigureCabinet(builder);
        }

        static private void ConfigureNewsEntry(ModelBuilder builder)
        {
            builder.Entity<NewsEntry>()
                .HasMany(n => n.Comments)
                .WithOne(c => c.NewsEntry)
                .OnDelete(DeleteBehavior.Cascade);
        }

        static private void ConfigureCabinet(ModelBuilder builder)
        {
            builder.Entity<Cabinet>()
                .HasMany(c => c.Events)
                .WithOne(e => e.Cabinet);
        }

        static private void ConfigureChat(ModelBuilder builder)
        {
            builder.Entity<Chat>()
                .HasMany(c => c.Participants)
                .WithMany(u => u.Chats)
                .UsingEntity(join => join.ToTable(_chatsParticipantsTableName));
            builder.Entity<Chat>()
                .HasMany(c => c.Messages)
                .WithOne(m => m.Chat)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Chat>()
                .HasOne(c => c.LastMessage)
                .WithOne();
        }

        static private void ConfigureAppUserRole(ModelBuilder builder)
        {
            builder.Entity<AppUserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRole)
                .HasForeignKey(ur => ur.RoleId);
            builder.Entity<AppUserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);
        }

        static private void ConfigureEvent(ModelBuilder builder)
        {
            builder.Entity<Event>()
                .HasMany(e => e.Listeners)
                .WithMany(u => u.ListeningEvents)
                .UsingEntity(join => join.ToTable(_eventsListenersTableName));
        }

        static private void ConfigureAppUser(ModelBuilder builder)
        {
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
                .UsingEntity(join => join.ToTable(_newsLikersTableName));
            builder.Entity<AppUser>()
                .HasMany(u => u.NewsEntries)
                .WithOne(n => n.Author)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<AppUser>()
                .HasMany(u => u.Subscribers)
                .WithMany(u => u.Subscriptions)
                .UsingEntity(join => join.ToTable(_subscribersSubscribtionsTableName));
            builder.Entity<AppUser>()
                .HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId);
        }
    }
}