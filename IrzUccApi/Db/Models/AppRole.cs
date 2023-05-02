using Microsoft.AspNetCore.Identity;

namespace IrzUccApi.Db.Models
{
    public class AppRole : IdentityRole<Guid>, IEntity
    {
        public virtual ICollection<AppUserRole> UserRole { get; set; } = new HashSet<AppUserRole>();
    }
}
