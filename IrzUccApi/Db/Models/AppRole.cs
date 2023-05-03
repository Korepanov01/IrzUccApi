using Microsoft.AspNetCore.Identity;

namespace IrzUccApi.Db.Models
{
    public class AppRole : IdentityRole<Guid>
    {
        public virtual ICollection<AppUserRole> UserRole { get; set; } = new HashSet<AppUserRole>();
    }
}
