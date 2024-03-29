﻿using Microsoft.AspNetCore.Identity;

namespace IrzUccApi.Db.Models
{
    public class AppUserRole : IdentityUserRole<Guid>, IEntity
    {
        public Guid Id { get; set; }
        public virtual AppUser User { get; set; } = new AppUser();
        public virtual AppRole Role { get; set; } = new AppRole();
    }
}
