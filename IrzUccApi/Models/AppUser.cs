﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace IrzUccApi.Models
{
    [Table("User")]
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public bool IsActiveAccount { get; set; } = true;
        public string? Patronymic { get; set; }
        public DateTime? EmploymentDate { get; set; }
        public DateTime Birthday { get; set; }
        public string? Image { get; set; }
        public string? AboutMyself { get; set; }
        public string? MyDoings { get; set; }
        public string? Skills { get; set; }
        public string? RefreshToken { get; set; }
        
        public virtual Position? Position { get; set; }
        public virtual ICollection<AppUser> Subscriptions { get; set; } = new HashSet<AppUser>();
        public virtual ICollection<AppUser> Subscribers { get; set; } = new HashSet<AppUser>();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<Event> MyEvents { get; set; } = new HashSet<Event>();
        public virtual ICollection<Event> ListeningEvents { get; set; } = new HashSet<Event>();
        public virtual ICollection<NewsEntry> LikedNewsEntries { get; set; } = new HashSet<NewsEntry>();
        public virtual ICollection<NewsEntry> MyNewsEntries { get; set; } = new HashSet<NewsEntry>();
    }
}
