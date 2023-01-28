using Microsoft.AspNetCore.Identity;

namespace IrzUccApi.Models.Db
{
    public class AppRole : IdentityRole
    {
        public virtual ICollection<AppUserRole> UserRole { get; set; } = new HashSet<AppUserRole>();
    }
}
