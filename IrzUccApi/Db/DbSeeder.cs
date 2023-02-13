using IrzUccApi.Enums;
using IrzUccApi.Models.Db;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi.Db
{
    public class DbSeeder
    {
        static private readonly string _superAdminEmail = "user@example.com";
        static private readonly string _superAdminPassword = "string";
        static private readonly string _superAdminName = "Главный";
        static private readonly string _superAdminSurname = "Администратор";


        private readonly ModelBuilder _modelBuilder;

        private Dictionary<string, AppRole> _appRoles = new();

        public DbSeeder(ModelBuilder builder)
        {
            _modelBuilder = builder;
        }

        public void SeedRequiredData()
        {
            foreach (var role in new[] { RolesNames.Admin, RolesNames.SuperAdmin, RolesNames.Support, RolesNames.CabinetsManager })
            {
                _appRoles.Add(role, new AppRole
                {
                    Name = role,
                    NormalizedName = role.ToUpper()
                });
            }
            _modelBuilder.Entity<AppRole>().HasData(_appRoles.Values);

            var superAdmin = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = _superAdminName,
                Surname = _superAdminSurname,
                UserName = _superAdminEmail,
                NormalizedUserName = _superAdminEmail.ToUpper(),
                Email = _superAdminEmail,
                NormalizedEmail = _superAdminEmail.ToUpper(),
            };
            superAdmin.PasswordHash = new PasswordHasher<AppUser>().HashPassword(superAdmin, _superAdminPassword);
            _modelBuilder.Entity<AppUser>().HasData(superAdmin);

            var superAdminUserRoles = _appRoles.Select(r => new AppUserRole
            {
                RoleId = r.Value.Id,
                UserId = superAdmin.Id
            });
            _modelBuilder.Entity<AppUserRole>().HasData(superAdminUserRoles);
        }

        public void SeedTestData()
        {
            var positionsNames = new[] 
            { 
                "Сотрудник поддержки", 
                "Сотрудник канцелярии", 
                "Рабочий", 
                "Администратор ЕЦК", 
                "Сторож" 
            };

            var positions = new Dictionary<string, Position>();
            foreach (var positionName in positionsNames) 
                positions.Add(positionName, new Position 
                { 
                    Id = Guid.NewGuid(),
                    Name = positionName 
                });
            _modelBuilder.Entity<Position>().HasData(positions.Values);

            var usersEmails = new[] { "ivan@irz.ru", "sergey@irz.ru", "ostalf@irz.ru" };
            var usersFirstNames = new[] { "Иван", "Сергей", "Остальф" };
            var usersSurnames = new[] { "Иванов", "Сергеев", "Шольц" };
            var usersPatronymics = new[] { "Иванович", "Сергеевич", null };
            var usersBirthdays = new[] { new DateTime(1956, 2, 1).ToUniversalTime(), new DateTime(1962, 3, 5).ToUniversalTime(), new DateTime(1970, 7, 23).ToUniversalTime() };
            var usersAboutMyselfs = new[] { "Простой рабочий человек", "Начальник цеха", null };
            var usersMyDoings = new[] { "Люблю вырезать фигурки из дерева", null, null };
            var usersSkills = new[] { "Токарь 5 разряда", null, null };
            var users = new Dictionary<string, AppUser>();
            for(int i = 0; i < usersEmails.Length; i++)
                users.Add(usersEmails[i], new AppUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = usersEmails[i],
                    NormalizedEmail = usersEmails[i].ToUpper(),
                    UserName = usersEmails[i].ToUpper(),
                    NormalizedUserName = usersEmails[i].ToUpper(),
                    FirstName = usersFirstNames[i],
                    Surname = usersSurnames[i],
                    Patronymic = usersPatronymics[i],
                    Birthday = usersBirthdays[i],
                    AboutMyself = usersAboutMyselfs[i],
                    MyDoings = usersMyDoings[i],
                    Skills = usersSkills[i]
                });
            foreach(var user in users)
            {
                user.Value.PasswordHash = new PasswordHasher<AppUser>().HashPassword(user.Value, "string");
            }
            _modelBuilder.Entity<AppUser>().HasData(users.Values);

            var userRoles = new[]
            {
                new AppUserRole
                {
                    UserId = users["sergey@irz.ru"].Id,
                    RoleId = _appRoles[RolesNames.Support].Id
                },
                new AppUserRole
                {
                    UserId = users["ostalf@irz.ru"].Id,
                    RoleId = _appRoles[RolesNames.CabinetsManager].Id
                },
                new AppUserRole
                {
                    UserId = users["ostalf@irz.ru"].Id,
                    RoleId = _appRoles[RolesNames.Admin].Id
                }
            };
            _modelBuilder.Entity<AppUserRole>().HasData(userRoles);
        }
    }
}