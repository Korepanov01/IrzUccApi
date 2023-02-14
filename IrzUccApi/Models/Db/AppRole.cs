using Microsoft.AspNetCore.Identity;

namespace IrzUccApi.Models.Db
{
    public class AppRole : IdentityRole<Guid>
    {
        public virtual ICollection<AppUserRole> UserRole { get; set; } = new HashSet<AppUserRole>();
    }
}
