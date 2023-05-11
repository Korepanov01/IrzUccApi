using Microsoft.AspNetCore.Identity;

namespace IrzUccApi.Db.Models
{
    public class AppRole : IdentityRole<Guid>, IEntity
    {
        public virtual ISet<AppUserRole> UserRole { get; set; } = new HashSet<AppUserRole>();
    }
}
