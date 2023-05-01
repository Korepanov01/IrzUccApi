using Microsoft.AspNetCore.Identity;

namespace IrzUccApi.Models.Db
{
    public class AppRole : IdentityRole<Guid>, IEntity
    {
        public virtual ICollection<AppUserRole> UserRole { get; set; } = new HashSet<AppUserRole>();
    }
}
