using Microsoft.EntityFrameworkCore;

namespace IrzUccApi
{
    public class ApplicationContext: DbContext
    {
        const string CONNECTION_STRING = "Host=localhost;Port=5432;Database=usersdb;Username=postgres;Password=пароль_от_postgres";

        public ApplicationContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(CONNECTION_STRING);
        }
    }
}
