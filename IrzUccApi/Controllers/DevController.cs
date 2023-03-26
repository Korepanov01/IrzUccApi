﻿using IrzUccApi.Db;
using IrzUccApi.Enums;
using IrzUccApi.Models.Db;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;

namespace IrzUccApi.Controllers
{
    [Route("api/dev")]
    [ApiController]
    public class DevController : ControllerBase
    {
        const string RandomText = "Таким образом постоянное информационно-пропагандистское обеспечение нашей деятельности позволяет оценить значение модели развития. С другой стороны сложившаяся структура организации обеспечивает широкому кругу (специалистов) участие в формировании соответствующий условий активизации.\r\n\r\nРавным образом дальнейшее развитие различных форм деятельности способствует подготовки и реализации позиций, занимаемых участниками в отношении поставленных задач. Повседневная практика показывает, что начало повседневной работы по формированию позиции требуют от нас анализа модели развития.\r\n\r\nЗначимость этих проблем настолько очевидна, что укрепление и развитие структуры требуют определения и уточнения форм развития. Таким образом постоянное информационно-пропагандистское обеспечение нашей деятельности представляет собой интересный эксперимент проверки позиций, занимаемых участниками в отношении поставленных задач.\r\n\r\nРавным образом постоянный количественный рост и сфера нашей активности позволяет выполнять важные задания по разработке модели развития. Разнообразный и богатый опыт постоянное информационно-пропагандистское обеспечение нашей деятельности влечет за собой процесс внедрения и модернизации систем массового участия. Равным образом сложившаяся структура организации требуют определения и уточнения направлений прогрессивного развития. Задача организации, в особенности же рамки и место обучения кадров позволяет выполнять важные задания по разработке существенных финансовых и административных условий.\r\n\r\nТоварищи! постоянное информационно-пропагандистское обеспечение нашей деятельности обеспечивает широкому кругу (специалистов) участие в формировании дальнейших направлений развития. Значимость этих проблем настолько очевидна, что новая модель организационной деятельности представляет собой интересный эксперимент проверки модели развития. Идейные соображения высшего порядка, а также сложившаяся структура организации в значительной степени обуславливает создание дальнейших направлений развития. Идейные соображения высшего порядка, а также укрепление и развитие структуры играет важную роль в формировании системы обучения кадров, соответствует насущным потребностям. Товарищи! новая модель организационной деятельности требуют от нас анализа позиций, занимаемых участниками в отношении поставленных задач. С другой стороны новая модель организационной деятельности позволяет оценить значение направлений прогрессивного развития.\r\n\r\nПовседневная практика показывает, что дальнейшее развитие различных форм деятельности влечет за собой процесс внедрения и модернизации существенных финансовых и административных условий. Повседневная практика показывает, что рамки и место обучения кадров требуют от нас анализа форм развития. Задача организации, в особенности же сложившаяся структура организации позволяет оценить значение соответствующий условий активизации.\r\n\r\nПовседневная практика показывает, что постоянное информационно-пропагандистское обеспечение нашей деятельности в значительной степени обуславливает создание существенных финансовых и административных условий. С другой стороны постоянный количественный рост и сфера нашей активности способствует подготовки и реализации форм развития. Разнообразный и богатый опыт начало повседневной работы по формированию позиции позволяет оценить значение позиций, занимаемых участниками в отношении поставленных задач. Идейные соображения высшего порядка, а также новая модель организационной деятельности обеспечивает широкому кругу (специалистов) участие в формировании существенных финансовых и административных условий. Не следует, однако забывать, что укрепление и развитие структуры влечет за собой процесс внедрения и модернизации новых предложений.\r\n\r\nС другой стороны постоянное информационно-пропагандистское обеспечение нашей деятельности в значительной степени обуславливает создание новых предложений. Идейные соображения высшего порядка, а также консультация с широким активом представляет собой интересный эксперимент проверки системы обучения кадров, соответствует насущным потребностям. Повседневная практика показывает, что реализация намеченных плановых заданий играет важную роль в формировании направлений прогрессивного развития.\r\n\r\nИдейные соображения высшего порядка, а также рамки и место обучения кадров влечет за собой процесс внедрения и модернизации модели развития. Значимость этих проблем настолько очевидна, что консультация с широким активом требуют от нас анализа систем массового участия. Таким образом постоянный количественный рост и сфера нашей активности требуют определения и уточнения направлений прогрессивного развития. Товарищи! новая модель организационной деятельности обеспечивает широкому кругу (специалистов) участие в формировании направлений прогрессивного развития. Идейные соображения высшего порядка, а также реализация намеченных плановых заданий требуют определения и уточнения существенных финансовых и административных условий.\r\n\r\nИдейные соображения высшего порядка, а также постоянное информационно-пропагандистское обеспечение нашей деятельности играет важную роль в формировании направлений прогрессивного развития. Не следует, однако забывать, что консультация с широким активом требуют определения и уточнения дальнейших направлений развития. Разнообразный и богатый опыт укрепление и развитие структуры позволяет выполнять важные задания по разработке соответствующий условий активизации. Равным образом укрепление и развитие структуры позволяет оценить значение систем массового участия.";
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

            await SeedUserPositionsAsync(users, positions);

            await SeedSubscriptionsAsync(users);

            var news = await SeedNewsAsync(users);

            await SeedLikesAsync(users, news);

            await SeedCommentsAsync(users, news);
        }

        [HttpPost("seed_random_news")]
        public async Task SeedRandomAsync(int count = 10)
        {
            var users = await _dbContext.Users.ToArrayAsync();

            var random = new Random();
            for (var i = 0; i < count; i++)
            {
                var author = users[random.Next(users.Length)];

                var titleInd = random.Next(0, RandomText.Length - 50);
                var textInd = random.Next(0, RandomText.Length - 400);

                var randomNewsEntry = new NewsEntry
                {
                    Author = author,
                    Title = RandomText[titleInd..(titleInd + random.Next(20, 50))],
                    Text = RandomText[textInd..(textInd + random.Next(100, 400))],
                    DateTime = new DateTime(2023, random.Next(1, 12), random.Next(1, 28)).ToUniversalTime(),
                    IsPublic = author.UserRoles.Select(ur => ur.Role.Name).Contains(RolesNames.Support) && random.Next(3) == 0,
                };
                await _dbContext.AddAsync(randomNewsEntry);

                foreach(var user in users)
                {
                    if(random.Next(3) == 0)
                    {
                        randomNewsEntry.Likers.Add(user);
                    }

                    var commentTextInd = random.Next(0, RandomText.Length - 200);
                    if (random.Next(3) == 0)
                    {
                        var randomComment = new Comment
                        {
                            Author = user,
                            Text = RandomText[commentTextInd..(commentTextInd + random.Next(200))],
                            DateTime = randomNewsEntry.DateTime.AddDays(random.Next(7))
                        };
                        await _dbContext.AddAsync(randomNewsEntry);
                        randomNewsEntry.Comments.Add(randomComment);
                    }
                }
            }
            await _dbContext.SaveChangesAsync();
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

        private async Task SeedUserPositionsAsync(Dictionary<string, AppUser> users, Dictionary<string, Position> positions)
        {
            var userPositions = new[]
            {
                new UserPosition
                {
                    User = users["ivan@irz.ru"],
                    Position = positions["Сторож"],
                    Start = new DateTime(1992, 3, 5).ToUniversalTime(),
                    End = new DateTime(2001, 3, 5).ToUniversalTime(),
                },
                new UserPosition
                {
                    User = users["ivan@irz.ru"],
                    Position = positions["Рабочий"],
                    Start = new DateTime(2001, 3, 5).ToUniversalTime(),
                },
                new UserPosition
                {
                    User = users["sergey@irz.ru"],
                    Position = positions["Сотрудник поддержки"],
                    Start = new DateTime(2001, 3, 5).ToUniversalTime(),
                },
                new UserPosition
                {
                    User = users["ostalf@irz.ru"],
                    Position = positions["Администратор ЕЦК"],
                    Start = new DateTime(2001, 3, 5).ToUniversalTime(),
                },
                new UserPosition
                {
                    User = users["ostalf@irz.ru"],
                    Position = positions["Сотрудник канцелярии"],
                    Start = new DateTime(2003, 3, 5).ToUniversalTime(),
                },
            };
            await _dbContext.AddRangeAsync(userPositions);
            await _dbContext.SaveChangesAsync();
        }

        private async Task SeedSubscriptionsAsync(Dictionary<string, AppUser> users)
        {
            users["sergey@irz.ru"].Subscribers.Add(users["ivan@irz.ru"]);
            users["ostalf@irz.ru"].Subscribers.Add(users["ivan@irz.ru"]);
            users["ostalf@irz.ru"].Subscribers.Add(users["sergey@irz.ru"]);

            _dbContext.UpdateRange(users.Values);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<Dictionary<string, NewsEntry>> SeedNewsAsync(Dictionary<string, AppUser> users)
        {
            var newsEntries = new[]
            {
                new NewsEntry
                {
                    Author = users["ivan@irz.ru"],
                    Title = "Заголовок 1",
                    Text = "Не следует, однако забывать, что сложившаяся структура организации позволяет выполнять важные задания по разработке направлений прогрессивного развития. С другой стороны постоянное информационно-пропагандистское обеспечение нашей деятельности представляет собой интересный эксперимент проверки модели развития. Задача организации, в особенности же постоянный количественный рост и сфера нашей активности требуют от нас анализа форм развития. Таким образом рамки и место обучения кадров позволяет оценить значение модели развития.\r\n\r\nРавным образом укрепление и развитие структуры влечет за собой процесс внедрения и модернизации соответствующий условий активизации. Повседневная практика показывает, что дальнейшее развитие различных форм деятельности способствует подготовки и реализации модели развития. Таким образом рамки и место обучения кадров требуют определения и уточнения форм развития.\r\n\r\nТаким образом новая модель организационной деятельности играет важную роль в формировании системы обучения кадров, соответствует насущным потребностям. Задача организации, в особенности же новая модель организационной деятельности позволяет выполнять важные задания по разработке форм развития. С другой стороны дальнейшее развитие различных форм деятельности представляет собой интересный эксперимент проверки новых предложений. Таким образом постоянное информационно-пропагандистское обеспечение нашей деятельности требуют определения и уточнения направлений прогрессивного развития. Разнообразный и богатый опыт начало повседневной работы по формированию позиции в значительной степени обуславливает создание системы обучения кадров, соответствует насущным потребностям.\r\n\r\n",
                    DateTime = new DateTime(2022, 1, 1).ToUniversalTime(),
                    IsPublic = false,
                },
                new NewsEntry
                {
                    Author = users["ivan@irz.ru"],
                    Title = "Заголовок 2",
                    Text = "Таким образом постоянное информационно-пропагандистское обеспечение нашей деятельности влечет за собой процесс внедрения и модернизации системы обучения кадров, соответствует насущным потребностям. С другой стороны сложившаяся структура организации требуют определения и уточнения системы обучения кадров, соответствует насущным потребностям. Разнообразный и богатый опыт дальнейшее развитие различных форм деятельности позволяет выполнять важные задания по разработке соответствующий условий активизации.\r\n\r\nЗадача организации, в особенности же дальнейшее развитие различных форм деятельности влечет за собой процесс внедрения и модернизации новых предложений. Равным образом начало повседневной работы по формированию позиции обеспечивает широкому кругу (специалистов) участие в формировании дальнейших направлений развития. Равным образом рамки и место обучения кадров способствует подготовки и реализации модели развития. Таким образом дальнейшее развитие различных форм деятельности способствует подготовки и реализации системы обучения кадров, соответствует насущным потребностям. Товарищи! постоянный количественный рост и сфера нашей активности требуют от нас анализа систем массового участия.\r\n\r\nЗначимость этих проблем настолько очевидна, что постоянный количественный рост и сфера нашей активности в значительной степени обуславливает создание форм развития. Идейные соображения высшего порядка, а также укрепление и развитие структуры требуют от нас анализа систем массового участия. Разнообразный и богатый опыт новая модель организационной деятельности в значительной степени обуславливает создание модели развития. С другой стороны рамки и место обучения кадров позволяет выполнять важные задания по разработке модели развития. С другой стороны постоянное информационно-пропагандистское обеспечение нашей деятельности позволяет выполнять важные задания по разработке систем массового участия.\r\n\r\n",
                    DateTime = new DateTime(2022, 2, 1).ToUniversalTime(),
                    IsPublic = false,
                },
                new NewsEntry
                {
                    Author = users["sergey@irz.ru"],
                    Title = "Заголовок 3",
                    Text = "Таким образом начало повседневной работы по формированию позиции способствует подготовки и реализации модели развития. Не следует, однако забывать, что дальнейшее развитие различных форм деятельности способствует подготовки и реализации дальнейших направлений развития. Не следует, однако забывать, что рамки и место обучения кадров позволяет оценить значение направлений прогрессивного развития.\r\n\r\nТоварищи! укрепление и развитие структуры обеспечивает широкому кругу (специалистов) участие в формировании позиций, занимаемых участниками в отношении поставленных задач. Разнообразный и богатый опыт дальнейшее развитие различных форм деятельности требуют от нас анализа дальнейших направлений развития. С другой стороны реализация намеченных плановых заданий позволяет выполнять важные задания по разработке направлений прогрессивного развития. Задача организации, в особенности же рамки и место обучения кадров влечет за собой процесс внедрения и модернизации форм развития.\r\n\r\nРавным образом начало повседневной работы по формированию позиции обеспечивает широкому кругу (специалистов) участие в формировании существенных финансовых и административных условий. С другой стороны постоянное информационно-пропагандистское обеспечение нашей деятельности обеспечивает широкому кругу (специалистов) участие в формировании модели развития. Таким образом укрепление и развитие структуры влечет за собой процесс внедрения и модернизации позиций, занимаемых участниками в отношении поставленных задач. Значимость этих проблем настолько очевидна, что сложившаяся структура организации требуют определения и уточнения существенных финансовых и административных условий. Значимость этих проблем настолько очевидна, что новая модель организационной деятельности играет важную роль в формировании дальнейших направлений развития.\r\n\r\n",
                    DateTime = new DateTime(2022, 3, 1).ToUniversalTime(),
                    IsPublic = false,
                },
                new NewsEntry
                {
                    Author = users["sergey@irz.ru"],
                    Title = "Заголовок 4",
                    Text = "Не следует, однако забывать, что укрепление и развитие структуры требуют от нас анализа форм развития. Значимость этих проблем настолько очевидна, что новая модель организационной деятельности позволяет оценить значение дальнейших направлений развития.\r\n\r\nНе следует, однако забывать, что начало повседневной работы по формированию позиции способствует подготовки и реализации системы обучения кадров, соответствует насущным потребностям. Повседневная практика показывает, что рамки и место обучения кадров позволяет оценить значение модели развития. Задача организации, в особенности же укрепление и развитие структуры требуют определения и уточнения соответствующий условий активизации.\r\n\r\nРазнообразный и богатый опыт укрепление и развитие структуры позволяет выполнять важные задания по разработке позиций, занимаемых участниками в отношении поставленных задач. Значимость этих проблем настолько очевидна, что постоянное информационно-пропагандистское обеспечение нашей деятельности требуют определения и уточнения существенных финансовых и административных условий. Не следует, однако забывать, что постоянный количественный рост и сфера нашей активности представляет собой интересный эксперимент проверки новых предложений. Значимость этих проблем настолько очевидна, что сложившаяся структура организации позволяет выполнять важные задания по разработке дальнейших направлений развития. Равным образом рамки и место обучения кадров способствует подготовки и реализации системы обучения кадров, соответствует насущным потребностям.\r\n\r\n",
                    DateTime = new DateTime(2022, 3, 3).ToUniversalTime(),
                    IsPublic = true,
                },
                new NewsEntry
                {
                    Author = users["sergey@irz.ru"],
                    Title = "Заголовок 5",
                    Text = "Идейные соображения высшего порядка, а также укрепление и развитие структуры играет важную роль в формировании новых предложений. Равным образом дальнейшее развитие различных форм деятельности обеспечивает широкому кругу (специалистов) участие в формировании системы обучения кадров, соответствует насущным потребностям. Равным образом постоянное информационно-пропагандистское обеспечение нашей деятельности позволяет оценить значение направлений прогрессивного развития.\r\n\r\nРазнообразный и богатый опыт укрепление и развитие структуры играет важную роль в формировании позиций, занимаемых участниками в отношении поставленных задач. Товарищи! рамки и место обучения кадров позволяет выполнять важные задания по разработке систем массового участия. Задача организации, в особенности же дальнейшее развитие различных форм деятельности способствует подготовки и реализации системы обучения кадров, соответствует насущным потребностям. Таким образом постоянное информационно-пропагандистское обеспечение нашей деятельности позволяет выполнять важные задания по разработке позиций, занимаемых участниками в отношении поставленных задач. Задача организации, в особенности же постоянный количественный рост и сфера нашей активности влечет за собой процесс внедрения и модернизации существенных финансовых и административных условий.\r\n\r\nТаким образом рамки и место обучения кадров требуют от нас анализа системы обучения кадров, соответствует насущным потребностям. Товарищи! дальнейшее развитие различных форм деятельности требуют определения и уточнения модели развития. Равным образом новая модель организационной деятельности позволяет выполнять важные задания по разработке систем массового участия. Значимость этих проблем настолько очевидна, что рамки и место обучения кадров в значительной степени обуславливает создание новых предложений. Не следует, однако забывать, что постоянное информационно-пропагандистское обеспечение нашей деятельности представляет собой интересный эксперимент проверки позиций, занимаемых участниками в отношении поставленных задач. Равным образом консультация с широким активом способствует подготовки и реализации модели развития.\r\n\r\n",
                    DateTime = new DateTime(2022, 4, 1).ToUniversalTime(),
                    IsPublic = true,
                },
                new NewsEntry
                {
                    Author = users["ostalf@irz.ru"],
                    Title = "Заголовок 6",
                    Text = "Таким образом новая модель организационной деятельности позволяет выполнять важные задания по разработке систем массового участия. Не следует, однако забывать, что начало повседневной работы по формированию позиции в значительной степени обуславливает создание существенных финансовых и административных условий. Повседневная практика показывает, что укрепление и развитие структуры обеспечивает широкому кругу (специалистов) участие в формировании позиций, занимаемых участниками в отношении поставленных задач. Разнообразный и богатый опыт рамки и место обучения кадров в значительной степени обуславливает создание направлений прогрессивного развития. Повседневная практика показывает, что начало повседневной работы по формированию позиции представляет собой интересный эксперимент проверки систем массового участия. С другой стороны начало повседневной работы по формированию позиции представляет собой интересный эксперимент проверки существенных финансовых и административных условий.\r\n\r\nЗадача организации, в особенности же постоянный количественный рост и сфера нашей активности представляет собой интересный эксперимент проверки форм развития. Идейные соображения высшего порядка, а также консультация с широким активом влечет за собой процесс внедрения и модернизации новых предложений. Товарищи! новая модель организационной деятельности в значительной степени обуславливает создание систем массового участия. Равным образом постоянное информационно-пропагандистское обеспечение нашей деятельности позволяет оценить значение существенных финансовых и административных условий. Значимость этих проблем настолько очевидна, что рамки и место обучения кадров представляет собой интересный эксперимент проверки направлений прогрессивного развития.",
                    DateTime = new DateTime(2022, 5, 1).ToUniversalTime(),
                    IsPublic = false,
                }
            };

            await _dbContext.AddRangeAsync(newsEntries);
            await _dbContext.SaveChangesAsync();

            return newsEntries.ToDictionary(n => n.Title);
        }

        private async Task SeedLikesAsync(Dictionary<string, AppUser> users, Dictionary<string, NewsEntry> news)
        {
            news["Заголовок 1"].Likers.Add(users["ivan@irz.ru"]);
            news["Заголовок 1"].Likers.Add(users["sergey@irz.ru"]);
            news["Заголовок 1"].Likers.Add(users["ostalf@irz.ru"]);

            news["Заголовок 4"].Likers.Add(users["ivan@irz.ru"]);
            news["Заголовок 4"].Likers.Add(users["ostalf@irz.ru"]);

            news["Заголовок 6"].Likers.Add(users["ostalf@irz.ru"]);

            _dbContext.UpdateRange(news.Values);
            await _dbContext.SaveChangesAsync();
        }

        private async Task SeedCommentsAsync(Dictionary<string, AppUser> users, Dictionary<string, NewsEntry> news)
        {
            var comments = new[]
            {
                new Comment
                {
                    NewsEntry = news["Заголовок 1"],
                    Author = users["sergey@irz.ru"],
                    Text = "Товарищи! сложившаяся структура организации требуют от нас анализа форм развития. С другой стороны новая модель организационной деятельности требуют от нас анализа позиций, занимаемых участниками в отношении поставленных задач. Не следует, однако забывать, что начало повседневной работы по формированию позиции требуют определения и уточнения новых предложений. Задача организации, в особенности же постоянное информационно-пропагандистское обеспечение нашей деятельности представляет собой интересный эксперимент проверки существенных финансовых и административных условий."
                },
                new Comment
                {
                    NewsEntry = news["Заголовок 4"],
                    Author = users["sergey@irz.ru"],
                    Text = "Повседневная практика показывает, что рамки и место обучения кадров играет важную роль в формировании системы обучения кадров, соответствует насущным потребностям. Разнообразный и богатый опыт постоянный количественный рост и сфера нашей активности обеспечивает широкому кругу (специалистов) участие в формировании систем массового участия. Разнообразный и богатый опыт дальнейшее развитие различных форм деятельности влечет за собой процесс внедрения и модернизации существенных финансовых и административных условий. Таким образом постоянный количественный рост и сфера нашей активности способствует подготовки и реализации дальнейших направлений развития. Товарищи! начало повседневной работы по формированию позиции позволяет выполнять важные задания по разработке дальнейших направлений развития. Повседневная практика показывает, что рамки и место обучения кадров позволяет оценить значение позиций, занимаемых участниками в отношении поставленных задач.\r\n\r\n"
                },
                new Comment
                {
                    NewsEntry = news["Заголовок 4"],
                    Author = users["sergey@irz.ru"],
                    Text = "Значимость этих проблем настолько очевидна, что дальнейшее развитие различных форм деятельности играет важную роль в формировании модели развития. Разнообразный и богатый опыт реализация намеченных плановых заданий в значительной степени обуславливает создание дальнейших направлений развития. Равным образом укрепление и развитие структуры требуют от нас анализа направлений прогрессивного развития. Таким образом новая модель организационной деятельности влечет за собой процесс внедрения и модернизации соответствующий условий активизации."
                },
            };

            await _dbContext.AddRangeAsync(comments);
            await _dbContext.SaveChangesAsync();
        }
    }
}
