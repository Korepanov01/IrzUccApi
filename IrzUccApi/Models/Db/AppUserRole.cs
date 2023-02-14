using Microsoft.AspNetCore.Identity;

namespace IrzUccApi.Models.Db
{
    public class AppUserRole : IdentityUserRole<Guid>
    {
        public virtual AppUser? User { get; set; }
        public virtual AppRole? Role { get; set; }
    }
}
