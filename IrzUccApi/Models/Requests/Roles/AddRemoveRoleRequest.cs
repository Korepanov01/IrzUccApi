using System.ComponentModel.DataAnnotations;

namespace IrzUccApi.Models.Requests.Role
{
    public class AddRemoveRoleRequest
    {
        [Required]
        public Guid UserId { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Role { get; set; } = string.Empty;
    }
}
