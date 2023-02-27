using IrzUccApi.Db;
using IrzUccApi.Enums;
using IrzUccApi.Models.Db;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi.Controllers
{
    [Route("api/dev")]
    [ApiController]
    public class DevController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public DevController(AppDbContext dbContext, UserManager<AppUser> userManager) 
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpPost("seed")] 
        public async Task Seed() 
        {
            var positions = await SeedPositionsAsync();
            
            var users = await SeedUsersAsync();

            var appRoles = await _dbContext.Roles.ToDictionaryAsync(r => r.Name);

            await SeedUserRolesAsync(users, appRoles);
        }

        private async Task<Dictionary<string, Position>> SeedPositionsAsync()
        {
            var positionsNames = new[]
            {
                "Сотрудник поддержки",
                "Сотрудник канцелярии",
                "Рабочий",
                "Администратор ЕЦК",
                "Сторож"
            };

            var positions = positionsNames.ToDictionary(posName => posName, posName => new Position
            {
                Name = posName
            });

            await _dbContext.AddRangeAsync(positions.Values);
            await _dbContext.SaveChangesAsync();

            return positions;
        }

        private async Task<Dictionary<string, AppUser>> SeedUsersAsync()
        {
            var password = "1Az$dsdwdadqwdqds";
            var users = new[]
            {
                new AppUser
                {
                    Email = "ivan@irz.ru",
                    UserName =  "ivan@irz.ru",
                    FirstName = "Иван",
                    Surname = "Иванов",
                    Patronymic = "Иванович",
                    Birthday = new DateTime(1956, 2, 1).ToUniversalTime(),
                    AboutMyself = "Простой рабочий человек",
                    MyDoings = "Люблю вырезать фигурки из дерева",
                    Skills = "Токарь 5 разряда",
                },
                new AppUser
                {
                    Email = "sergey@irz.ru",
                    UserName =  "sergey@irz.ru",
                    FirstName = "Сергей",
                    Surname = "Сергеев",
                    Patronymic = "Сергеевич",
                    Birthday = new DateTime(1962, 3, 5).ToUniversalTime(),
                    AboutMyself = "Начальник цеха",
                },
                new AppUser
                {
                    Email = "ostalf@irz.ru",
                    UserName =  "ostalf@irz.ru",
                    FirstName = "Остальф",
                    Surname = "Шольц",
                    Birthday = new DateTime(1970, 7, 23).ToUniversalTime(),
                },
            };
                        
            foreach (var user in users)
            {
                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                    throw new Exception(string.Join(' ', result.Errors.Select(e => e.Description)));
            }

            return users.ToDictionary(u => u.Email);
        }

        private async Task SeedUserRolesAsync(Dictionary<string, AppUser> users, Dictionary<string, AppRole> appRoles)
        {
            var userRoles = new[]
            {
                new AppUserRole
                {
                    User = users["sergey@irz.ru"],
                    Role = appRoles[RolesNames.Support]
                },
                new AppUserRole
                {
                    User = users["ostalf@irz.ru"],
                    Role = appRoles[RolesNames.CabinetsManager]
                },
                new AppUserRole
                {
                    User = users["ostalf@irz.ru"],
                    Role = appRoles[RolesNames.Admin]
                }
            };
            await _dbContext.AddRangeAsync(userRoles);
            await _dbContext.SaveChangesAsync();
        }
    }
}
