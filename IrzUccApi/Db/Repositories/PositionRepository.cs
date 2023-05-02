﻿using IrzUccApi.Db.Models;

namespace IrzUccApi.Db.Repositories
{
    public class PositionRepository : AppRepository<Position, AppDbContext>
    {
        public PositionRepository(AppDbContext dbContext) : base(dbContext) { }
    }
}
